#tool nuget:?package=Wyam&version=2.2.8
#addin nuget:?package=Cake.Wyam&version=2.2.8
#addin "nuget:?package=NetlifySharp&version=1.1.0"

using NetlifySharp;

var target = Argument("target", "Default");
var recipe = "Blog";
var theme = "SolidState";
var IsMainBranch = StringComparer.OrdinalIgnoreCase.Equals("refs/heads/main", GitHubActions.Environment.Workflow.Ref);

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
    .Does(() =>
    {
        var netlifyToken = EnvironmentVariable("NETLIFY_TOKEN");
        if(string.IsNullOrEmpty(netlifyToken))
        {
            throw new Exception("Could not get Netlify token environment variable");
        }

        Information("Deploying output to Netlify");
        var client = new NetlifyClient(netlifyToken);
        var outputPath = MakeAbsolute(Directory("./output")).FullPath;
        Information($"Output: {outputPath}");
        client.UpdateSiteAsync((string)MakeAbsolute(Directory("./output")).FullPath, "rodneylittlesii.netlify.com").GetAwaiter().GetResult();
    });

Task("Default")
    .IsDependentOn("Preview");

Task("GitHubActions")
    .IsDependentOn("Deploy");

RunTarget(target);