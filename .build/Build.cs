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
using Nuke.Common.Tooling;

[GitHubActions("ci",
    GitHubActionsImage.MacOsLatest,
    AutoGenerate = true,
    OnPushBranches = new[] {"main", "drafts", "draft/*"},
    OnPullRequestBranches = new[] {"drafts"},
    InvokedTargets = new[] {nameof(GitHubActions)},
    ImportSecrets = new[] {"NETLIFY_TOKEN", "NETLIFY_URL"})]
[DotNetVerbosityMapping]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Default);

    [GitRepository] public GitRepository GitRepository { get; }

    public AbsolutePath OutputDirectory => RootDirectory / "output";

    Target RestoreNuke => _ => _
        .OnlyWhenStatic(() => !IsLocalBuild)
        .Executes(() =>
        {
            DotNetTasks
                .DotNetToolUpdate(configuration =>
                    configuration
                        .SetPackageName("Nuke.GlobalTool")
                        .EnableGlobal()
                        .SetArgumentConfigurator(args => args.Add("--version={0}", "0.25.0-alpha0377")));
        });

    Target RestoreWyam => _ => _
        .Executes(() =>
        {
            DotNetTasks
                .DotNetToolUpdate(configuration =>
                    configuration
                        .SetPackageName("Wyam.Tool")
                        .EnableGlobal()
                        .SetArgumentConfigurator(args => args.Add("--version={0}", "2.2.9")));
        });

    Target Restore => _ => _
        .DependsOn(RestoreWyam)
        .DependsOn(RestoreNuke)
        .Executes();

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            Wyam.Common.Tracing.Trace.AddListener(new NukeTraceListener());
            Wyam.Common.Tracing.Trace.Level = SourceLevels.All;
            
            EngineBuilder
                .Create()
                .WithConfiguration(this)
                .Execute();
        });

    Target Preview => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            Wyam.Common.Tracing.Trace.AddListener(new NukeTraceListener());
            Wyam.Common.Tracing.Trace.Level = SourceLevels.All;
            PreviewServer
                .Preview(() =>
                        EngineBuilder
                            .Create()
                            .WithSetting(Keys.CleanOutputPath, false)
                            .WithSetting("Drafts", true)
                            .WithConfiguration(this),
                this);
        });

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
