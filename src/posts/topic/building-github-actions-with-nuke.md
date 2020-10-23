Title: Building with GitHub Actions with Nuke
Drafted: 10/12/2020
Published: 10/22/2020
Tags:
- Nuke
- Dev Ops
- GitHub Actions
- Continuous Delivery
---

### For more information on how to get started with Nuke check out [Georg Dangl](https://blog.dangl.me/archive/lets-use-nuke-to-quickly-deploy-an-app-to-azure-via-zip-deployment/) explain how to go from zero to deploy with Nuke.

## Time to break the build!

One of the biggest problems I face as a .NET developer on macOS is tooling.  It took until .NET Core 2.1 before I could build my blog on macOS, and when it comes to IntelliSense, and productivity your choices on mac are slim.  Tooling is important for writing code, but what about building code?!  Well, I have been playing with a build system called [Nuke Build](https://nuke.build), and now that GitHub Actions are maturing, and my blog is stored in a GitHub repository, I figured let's break our build.

So we are taking our build from using

- Cake
- Azure DevOps

to using

- Nuke
- GitHub Actions

## Adding Nuke Build into the mix
Nuke provides C# style intellisense out of the box in your IDE of choice because a Nuke build is a C# console application.

The major DSL differences are

```csharp
Task => Target
```

```csharp 
Does => Execute
```

Include dependencies in a csproj file instead of compiler directives.  Everything else is just API specifics. so I won't bore you with the noise.  Here is the result of the conversion.
```csharp
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
            var engine = new Engine();
            new WyamConfiguration(engine, this);
            engine.Execute();
        });

    Target Preview => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            Wyam.Common.Tracing.Trace.AddListener(new NukeTraceListener());
            Wyam.Common.Tracing.Trace.Level = SourceLevels.All;
            PreviewServer.Preview(
                () =>
                {
                    var engine = new Engine();
                    engine.Settings[Keys.CleanOutputPath] = false;
                    engine.Settings["Drafts"] = true;
                    new WyamConfiguration(engine, this);
                    return engine;
                },
                this
            );
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
```

## Yet Another Markdown Artifact
I converted easily enough from Cake as the concepts of all DSL's are similar.  You create domain-specific terms that invoke actions.  So once you understand the dialect of build, it becomes trivial to move.  What I didn't want to move was the build specific YAML.  This is always the most tedious part of a build process set up for me.  Constructing the YAML that executes my custom build script (because I always write a platform-agnostic solution).  So I ended up with some YAML like the following.

```yaml
name: Build Action
on:
  push:
    branches:
    - main
    - drafts
    - draft/*
  pull_request:
    branches: [ main, drafts ]
jobs:
  build:
    runs-on: macos-latest
    steps:
    - uses: actions/checkout@v2    
    - name: Setup .NET Core SDK
      uses: actions/setup-dotnet@v1.7.2
      with:
        dotnet-version: 3.1.x
    - name: Nuke Build
      shell: bash
      run: ./build.sh --target GitHubActions --verbosity verbose
      env:
        NETLIFY_TOKEN: ${{ secrets.NETLIFY_TOKEN }}
        NETLIFY_URL: ${{ secrets.NETLIFY_URL }}
```

#### The Secrets!

Even though I couldn't find anything explicitly in the documentation, seems the `secrets` in GitHub Actions on a single repo don't like being passed into a job.  So I had to pass them into the step.

## Nuke can generate YAML!
I dislike YAML.  Not because it is yet another markup language, it does serve a purpose.  I dislike YAML because it is an artifact I have to generate to execute my build script on my target CI systems. The glorious build script that I wrote specifically to not need anything but a command-line interface, now will not work without YAML.  Well, for my next trick I am going to take my Nuke build and build my GitHub Actions YAML pipeline with my build script.  Fortunately, it's as easy as adding a custom attribute provided by Nuke.  You just add an attribute for the given build system you want to build for and it will generate that build systems yaml.

```csharp
[GitHubActions("ci",
    GitHubActionsImage.MacOsLatest,
    AutoGenerate = true,
    OnPushBranches = new[] {"main", "drafts", "draft/*"},
    OnPullRequestBranches = new[] {"drafts"},
    InvokedTargets = new[] {nameof(GitHubActions)},
    ImportSecrets = new[] {"NETLIFY_TOKEN", "NETLIFY_URL"})]
```

The above attribute will generate the following YAML and output to the `./.github/workflows` folder.

```yaml
name: ci

on:
  push:
    branches:
      - main
      - drafts
      - draft/*
  pull_request:
    branches:
      - drafts

jobs:
  macOS-latest:
    name: macOS-latest
    runs-on: macOS-latest
    steps:
      - uses: actions/checkout@v1
      - name: Run './build.cmd GitHubActions'
        run: ./build.cmd GitHubActions
        env:
            NETLIFY_TOKEN: ${{ secrets.NETLIFY_TOKEN }}
            NETLIFY_URL: ${{ secrets.NETLIFY_URL }}
```

Now my agnostic build script can generate the YAML required to execute my GitHub Actions Workflow!  I no longer have to fiddle with YAML.  I can have my build script generate the YAML for me!  I was extremely skeptical of this feature when it was presented to me.  Now, that I am converting builds from Azure DevOps to GitHub Actions, I am finding this feature invaluable.  I don't have to convert YAML, I just have to add an attribute to my script and Nuke will build it for me!
