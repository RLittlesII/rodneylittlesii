using System;
using System.Linq;
using Wyam.Common.Tracing;
using System.Collections.Generic;
using Wyam.Hosting;
using Wyam.Common.IO;
using Nuke.Common;
using Wyam.Core.Execution;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using SourceLevels = System.Diagnostics.SourceLevels;
using Wyam.Common.Util;

internal static class PreviewServer
{
    public static void Preview(Func<Engine> engineFunc, NukeBuild build)
    {
        var messageEvent = new AutoResetEvent(false);
        var _changedFiles = new ConcurrentQueue<string>();
        var _exit = new InterlockedBool(false);
        // Start the preview server
        DirectoryPath previewPath = (NukeBuild.RootDirectory / "output").ToString();

        var previewServer = PreviewServer.Start(previewPath, 5080, false, null, true, new Dictionary<string, string>());
        var engine = engineFunc();

        try { engine.Execute(); } catch { }
        Trace.Information($"Preview server listening at http://localhost:{5080}");

        Trace.Information("Watching paths(s) {0}", string.Join(", ", engine.FileSystem.InputPaths));
        var inputFolderWatcher = new ActionFileSystemWatcher(
            engine.FileSystem.GetOutputDirectory().Path,
            engine.FileSystem.GetInputDirectories().Select(x => x.Path),
            true,
            "*.*",
            path =>
            {
                _changedFiles.Enqueue(path);
                messageEvent.Set();
            });

        // Start the message pump if an async process is running
        ExitCode exitCode = ExitCode.Normal;
        // Only wait for a key if console input has not been redirected, otherwise it's on the caller to exit
        if (!Console.IsInputRedirected)
        {
            // Start the key listening thread
            Thread thread = new Thread(() =>
            {
                Trace.Information("Hit Ctrl-C to exit");
                Console.TreatControlCAsInput = true;
                while (true)
                {
                    // Would have prefered to use Console.CancelKeyPress, but that bubbles up to calling batch files
                    // The (ConsoleKey)3 check is to support a bug in VS Code: https://github.com/Microsoft/vscode/issues/9347
                    ConsoleKeyInfo consoleKey = Console.ReadKey(true);
                    if (consoleKey.Key == (ConsoleKey)3 || (consoleKey.Key == ConsoleKey.C && (consoleKey.Modifiers & ConsoleModifiers.Control) != 0))
                    {
                        _exit.Set();
                        messageEvent.Set();
                        break;
                    }
                }
            })
            {
                IsBackground = true
            };
            thread.Start();
        }

        // Wait for activity
        while (true)
        {
            messageEvent.WaitOne(); // Blocks the current thread until a signal
            if (_exit)
            {
                break;
            }

            // Execute if files have changed
            HashSet<string> changedFiles = new HashSet<string>();
            string changedFile;
            while (_changedFiles.TryDequeue(out changedFile))
            {
                if (changedFiles.Add(changedFile))
                {
                    Trace.Verbose("{0} has changed", changedFile);
                }
            }
            if (changedFiles.Count > 0)
            {
                Trace.Information("{0} files have changed, re-executing", changedFiles.Count);
                engine = engineFunc();
                try { engine.Execute(); } catch { }
                previewServer?.TriggerReloadAsync().GetAwaiter().GetResult();
            }

            // Check one more time for exit
            if (_exit)
            {
                break;
            }
            Trace.Information("Hit Ctrl-C to exit");
            messageEvent.Reset();
        }

        // Shutdown
        Trace.Information("Shutting down");
        inputFolderWatcher?.Dispose();
        previewServer?.Dispose();
    }
    public static Server Start(DirectoryPath path, int port, bool forceExtension, DirectoryPath virtualDirectory, bool liveReload, IDictionary<string, string> contentTypes)
    {
        Server server;
        try
        {
            server = new Server(path.FullPath, port, !forceExtension, virtualDirectory?.FullPath, liveReload, contentTypes, Microsoft.Extensions.Logging.Abstractions.NullLoggerProvider.Instance);
            server.Start();
        }
        catch (Exception ex)
        {
            Trace.Critical($"Error while running preview server: {ex}");
            return null;
        }

        string urlPath = server.VirtualDirectory == null ? string.Empty : server.VirtualDirectory;
        Trace.Information($"Preview server listening at http://localhost:{port}{urlPath} and serving from path {path}"
            + (liveReload ? " with LiveReload support" : string.Empty));
        return server;
    }
}

/// <summary>
/// A wrapper around <see cref="FileSystemWatcher"/> that invokes a callback action on changes.
/// </summary>
internal class ActionFileSystemWatcher : IDisposable
{
    private readonly List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();
    private readonly string _outputPath;
    private readonly Action<string> _callback;

    public ActionFileSystemWatcher(DirectoryPath outputDirectory, IEnumerable<DirectoryPath> inputDirectories, bool includeSubdirectories, string filter, Action<string> callback)
    {
        foreach (string inputDirectory in inputDirectories.Select(x => x.Collapse().FullPath).Where(Directory.Exists))
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = inputDirectory,
                IncludeSubdirectories = includeSubdirectories,
                Filter = filter,
                EnableRaisingEvents = true
            };
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            _watchers.Add(watcher);
        }
        _outputPath = outputDirectory.Collapse().FullPath;
        _callback = callback;
    }

    public void Dispose()
    {
        foreach (FileSystemWatcher watcher in _watchers)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Changed -= OnChanged;
            watcher.Created -= OnChanged;
        }
    }

    private void OnChanged(object sender, FileSystemEventArgs args)
    {
        if (!args.FullPath.StartsWith(_outputPath, StringComparison.OrdinalIgnoreCase))
        {
            _callback(args.FullPath);
        }
    }
}
internal enum ExitCode
{
    Normal = 0,
    UnhandledError = 1,
    CommandLineError = 2,
    ConfigurationError = 3,
    ExecutionError = 4,
    UnsupportedRuntime = 5
}
internal class InterlockedBool
{
    private volatile int _set;

    public InterlockedBool()
    {
        _set = 0;
    }

    public InterlockedBool(bool initialState)
    {
        _set = initialState ? 1 : 0;
    }

    // Returns the previous switch state of the switch
    public bool Set()
    {
#pragma warning disable 420
        return Interlocked.Exchange(ref _set, 1) != 0;
#pragma warning restore 420
    }

    // Returns the previous switch state of the switch
    public bool Unset()
    {
#pragma warning disable 420
        return Interlocked.Exchange(ref _set, 0) != 0;
#pragma warning restore 420
    }

    // Returns the current state
    public static implicit operator bool(InterlockedBool interlockedBool)
    {
        return interlockedBool._set != 0;
    }
}
internal class TraceLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new TraceLogger(categoryName);

    public void Dispose()
    {
    }
}

internal class TraceLogger : ILogger
{
    private readonly string _categoryName;

    private static readonly Dictionary<LogLevel, SourceLevels> LevelMapping = new Dictionary<LogLevel, SourceLevels>
        {
            { LogLevel.Trace, SourceLevels.Verbose },
            { LogLevel.Debug, SourceLevels.Verbose },
            { LogLevel.Information, SourceLevels.Verbose },
            { LogLevel.Warning, SourceLevels.Warning },
            { LogLevel.Error, SourceLevels.Error },
            { LogLevel.Critical, SourceLevels.Critical },
            { LogLevel.None, SourceLevels.Off }
        };

    private static readonly Dictionary<LogLevel, Nuke.Common.LogLevel> TraceMapping = new Dictionary<LogLevel, Nuke.Common.LogLevel>
        {
            { LogLevel.Trace, Nuke.Common.LogLevel.Trace },
            { LogLevel.Debug, Nuke.Common.LogLevel.Trace },
            { LogLevel.Information, Nuke.Common.LogLevel.Normal },
            { LogLevel.Warning, Nuke.Common.LogLevel.Warning },
            { LogLevel.Error, Nuke.Common.LogLevel.Error },
            { LogLevel.Critical, Nuke.Common.LogLevel.Error },
            { LogLevel.None, Nuke.Common.LogLevel.Trace }
        };

    public TraceLogger(string categoryName)
    {
        _categoryName = categoryName;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (IsEnabled(logLevel))
        {
            Logger.Log(TraceMapping[logLevel], $"{_categoryName}: {formatter(state, exception)}");
        }
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) => EmptyDisposable.Instance;
}
