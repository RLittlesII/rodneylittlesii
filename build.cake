#tool nuget:?package=Wyam&version=2.2.8
#addin nuget:?package=Cake.Wyam&version=2.2.8
#addin "nuget:?package=NetlifySharp&version=0.1.0"

using NetlifySharp;

var target = Argument("target", "Default");
var recipe = "Blog";
var theme = "SolidState";
var IsMasterBranch = StringComparer.OrdinalIgnoreCase.Equals("main", AzurePipelines.Environment.Repository.SourceBranchName);

Setup(context =>
{
    Information(DateTime.Now);
    if(BuildSystem.IsRunningOnAzurePipelines)
    {
        IsMasterBranch = StringComparer.OrdinalIgnoreCase.Equals("main", AzurePipelines.Environment.Repository.SourceBranchName);
    }
    if(BuildSystem.IsRunningOnGitHubActions)
    {
        IsMasterBranch = StringComparer.OrdinalIgnoreCase.Equals("refs/heads/main", GitHubActions.Environment.Workflow.Ref);
    }
});

Teardown(context =>
{
    Information(DateTime.Now);	
});


Task("AzurePipelinesEnvironment")
    .WithCriteria(BuildSystem.IsRunningOnAzurePipelines)
    .Does(() => 
    {
    });

Task("GitHubActionsEnvironment")
    .WithCriteria(BuildSystem.IsRunningOnGitHubActions)
    .Does(() => 
    {
        Information("GITHUB_ACTION: {0}", GitHubActions.Environment.Workflow.Action);
        Information("GITHUB_ACTOR: {0}", GitHubActions.Environment.Workflow.Actor);
        Information("GITHUB_BASE_REF: {0}", GitHubActions.Environment.Workflow.BaseRef);
        Information("GITHUB_EVENT_NAME: {0}", GitHubActions.Environment.Workflow.EventName);
        Information("GITHUB_EVENT_PATH: {0}", GitHubActions.Environment.Workflow.EventPath);
        Information("GITHUB_HEAD_REF: {0}", GitHubActions.Environment.Workflow.HeadRef);
        Information("GITHUB_JOB: {0}", GitHubActions.Environment.Workflow.Job);
        Information("GITHUB_REPOSITORY: {0}", GitHubActions.Environment.Workflow.Repository);
        Information("GITHUB_REF: {0}", GitHubActions.Environment.Workflow.Ref);
        Information("GITHUB_SHA: {0}", GitHubActions.Environment.Workflow.Sha);
        Information("GITHUB_WORKFLOW: {0}", GitHubActions.Environment.Workflow.Workflow);
    });


Task("Environment")
    .IsDependentOn("AzurePipelinesEnvironment")
    .IsDependentOn("GitHubActionsEnvironment")
    .Does(() =>
    {
        Information("NETLIFY_TOKEN: {0}", EnvironmentVariable("NETLIFY_TOKEN") != null);        
    });

Task("Build")
    .IsDependentOn("Environment")
    .Does(() =>
    {
        Wyam(new WyamSettings
        {
            Recipe = recipe,
            Theme = theme,
            UpdatePackages = true
        });
    });

Task("Preview")
    .Does(() =>
    {
        Wyam(new WyamSettings
        {
            Recipe = recipe,
            Theme = theme,
            UpdatePackages = true,
            Preview = true,
            Watch = true,
            Settings = new Dictionary<string, object>() 
            {
                { "Drafts", true }
            }, 
            InputPaths = new DirectoryPath[] { "src", "drafts" }
        });
    });

Task("Deploy")
    .IsDependentOn("Build")
    .WithCriteria(IsMasterBranch)
    .Does(() =>
    {
        var netlifyToken = EnvironmentVariable("NETLIFY_TOKEN");
        if(string.IsNullOrEmpty(netlifyToken))
        {
            throw new Exception("Could not get Netlify token environment variable");
        }

        Information("Deploying output to Netlify");
        var client = new NetlifyClient(netlifyToken);
        client.UpdateSite($"rodneylittlesii.netlify.com", MakeAbsolute(Directory("./output")).FullPath).SendAsync().Wait();
    });

Task("Default")
    .IsDependentOn("Preview");

Task("AppVeyor")
    .IsDependentOn("Build");

Task("AzureDevOps")
    .IsDependentOn("Deploy");

Task("GitHubActions")
    .IsDependentOn("Deploy");

RunTarget(target);