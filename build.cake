#tool nuget:?package=Wyam&version=2.2.8
#addin nuget:?package=Cake.Wyam&version=2.2.8
#addin "nuget:?package=NetlifySharp&version=0.1.0"

using NetlifySharp;

var target = Argument("target", "Default");
var recipe = "Blog";
var theme = "SolidState";
var IsMasterBranch = StringComparer.OrdinalIgnoreCase.Equals("main", TFBuild.Environment.Repository.Branch);

Setup(context =>
{
    Information(DateTime.Now);
});

Teardown(context =>
{
    Information(DateTime.Now);	
});

Task("Build")
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
    .IsDependentOn("Build");

RunTarget(target);