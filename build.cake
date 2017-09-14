#tool nuget:?package=Wyam
#addin nuget:?package=Cake.Wyam

var target = Argument("target", "Default");
var recipe = "Blog";
var theme = "SolidState";
var IsMasterBranch = StringComparer.OrdinalIgnoreCase.Equals("master", AppVeyor.Environment.Repository.Branch);

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
            InputPaths = new DirectoryPath[] { "src" }
        });
    });

Task("Deploy")
    .WithCriteria(IsMasterBranch)
    .Does(() =>
    {
        string token = EnvironmentVariable("NETLIFY_TOKEN");
        if(string.IsNullOrEmpty(token))
        {
            throw new Exception("Could not get NETLIFY_TOKEN environment variable");
        }

        string url = EnvironmentVariable("NETLIFY_URL");
        if(string.IsNullOrEmpty(url))
        {
            throw new Exception("Could not get NETLIFY_URL environment variable");
        }

        // Upload via curl and zip instead
        Zip("./output", "output.zip", "./output/**/*");
        StartProcess("curl", "--header \"Content-Type: application/zip\" --header \"Authorization: Bearer " + token + "\" --data-binary \"@output.zip\" --url " + url);
    });

Task("Default")
    .IsDependentOn("Preview");

Task("AppVeyor")
    .IsDependentOn("Build")
    .IsDependentOn("Deploy");

RunTarget(target);