using System;
using System.Diagnostics;
using System.Net.Http;
using NetlifySharp;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Wyam.Common.Meta;
using Wyam.Core.Execution;
using Nuke.Common.CI;
using Nuke.Common.Tooling;

[GitHubActions("ci", GitHubActionsImage.MacOsLatest, GitHubActionsImage.WindowsLatest, GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    On = new[] {GitHubActionsTrigger.Push},
    OnPushTags = new[] {"v*"},
    OnPushBranches = new[] {"master", "next"},
    OnPullRequestBranches = new[] {"master", "next"},
    InvokedTargets = new[] {nameof(Default)})]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Default);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitRepository] public GitRepository GitRepository { get; }

    /// <summary>
    /// The directory where packaged output should be placed (zip, webdeploy, etc)
    /// </summary>
    public AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetTasks
                .DotNetToolUpdate(configuration =>
                    configuration
                        .SetPackageName("Wyam.Tool")
                        .EnableGlobal()
                        .SetArgumentConfigurator(args => args.Add("--version={0}", "2.2.9")));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            Wyam.Common.Tracing.Trace.AddListener(new NukeTraceListener());
            Wyam.Common.Tracing.Trace.Level = SourceLevels.All;
            var engine = new Engine();
            new WyamConfiguration(engine, this);
            engine.Execute();
        });

    Target Preview => _ => _
        .DependsOn(Compile)
        .Executes(
            () =>
            {
                Wyam.Common.Tracing.Trace.AddListener(new NukeTraceListener());
                Wyam.Common.Tracing.Trace.Level = SourceLevels.All;
                PreviewServer.Preview(
                    () =>
                    {
                        var engine = new Engine();
                        engine.Settings[Keys.CleanOutputPath] = false;
                        new WyamConfiguration(engine, this);
                        return engine;
                    },
                    this
                );
            }
        );

    Target Deploy => _ => _
        .DependsOn(Compile)
        .OnlyWhenStatic(() => GitRepository.Branch == "refs/heads/main")
        .Executes(() =>
        {
            var netlifyToken = Environment.GetEnvironmentVariable("NETLIFY_TOKEN");
            if(string.IsNullOrEmpty(netlifyToken))
            {
                throw new Exception("Could not get Netlify token environment variable");
            }

            var netlifyUrl = Environment.GetEnvironmentVariable("NETLIFY_URL");

            Logger.Info("Deploying output to Netlify");

            var client = new NetlifyClient(netlifyToken, new HttpClient());
            client.UpdateSiteAsync(OutputDirectory, netlifyUrl).GetAwaiter().GetResult();
        });

    Target GitHubActions => _ => _
        .DependsOn(Deploy)
        .Executes();

    Target Default => _ => _
        .DependsOn(Preview)
        .Executes();
}
