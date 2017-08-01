#tool nuget:?package=Wyam
#addin nuget:?package=Cake.Wyam

var target = Argument("target", "Default");
var recipe = "Blog";
var theme = "SolidState";

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
                { "Drafts", false }
            }
        });
    });

Task("Deploy")
    .Does(() =>
    {
        string token = EnvironmentVariable("NETLIFY_TOKEN");
        if(string.IsNullOrEmpty(token))
        {
            throw new Exception("Could not get NETLIFY_TOKEN environment variable");
        }

        // This uses the Netlify CLI, but it hits the 200/min API rate limit
        // To use this, also need #addin "Cake.Npm"
        // Npm.Install(x => x.Package("netlify-cli"));
        // StartProcess(
        //    MakeAbsolute(File("./node_modules/.bin/netlify.cmd")),
        //    "deploy -p output -s daveaglick -t " + token);

        // Upload via curl and zip instead
        Zip("./output", "output.zip", "./output/**/*");
        StartProcess("curl", "--header \"Content-Type: application/zip\" --header \"Authorization: Bearer " + token + "\" --data-binary \"@output.zip\" --url https://api.netlify.com/api/v1/sites/rodneylittlesii.netlify.com//deploys");
    });

Task("Default")
    .IsDependentOn("Preview");

Task("AppVeyor")
    .IsDependentOn("Build")
    .IsDependentOn("Deploy");

RunTarget(target);