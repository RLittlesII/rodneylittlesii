Title: Using Cake.Fastlane to provision ios
Drafted: 03/08/2018
Published: 04/18/2018
Tags:
    - Cake
    - Fastlane
    - iOS
    - Xamarin
---

# Expiring Certificates
Oh no! It's that time of year again.  I have to generate iOS certificates and provisioning profiles!  I dread doing this because the process of generating these is so tedious.  I have multiple profiles per single app.  There are a specific set of steps.  If those steps aren't followed to the letter, I have to start again.  Then I have to test each one to verify that it is done correctly.

What ever shall I do?  Use [fastlane tools](https://fastlane.tools/)?  Obviously.  This suit of tools has save me personally over a hundred hours of my life.  Those are hours I can use to write more code (if your into that sort of thing), practice my kung fu, or spend time with the wife!  With the advent of [Cake Build](http://cakebuild.net) and it's amazing plugin system; [Cake.Fastlane](https://github.com/RLittlesII/Cake.Fastlane) allows me to script out my specific needs per application and generate all my iOS provisioning profiles in a few easy steps.

Imagine generating twelve iOS provisioning profiles with one command and being done in the time it takes to go grab a cup of coffee!  Doesn't that sound like something you want as a Xamarin iOS developer?  How many times have you been asked to add a new tester to an iOS build?  Or remove testers?  New iPhone, new provisioning profile.  The reduction in complexity alone is worth the investment in setup.

# Why does this matter?
I have a seperate app bundle per environment.  Each bundle points to a different environment API.  For most traditional apps thats at minimum:
- test
- staging
- prod

My apps go a bit further.  I have a bundle for local development, a development deployment, and a mock deployment that we use for disconnected testing and UI audits.  This is something that Kent Boogaart discusses [here](https://kent-boogaart.com/blog/disconnected-layers).  I've even used this technique creating API's that serve up data from an in memory list.  Lastly I have a seperate prod and store bundle so I can test my app in production before I push to the appstores.

As you can tell, with 4-5 bundles per application I easily get overwhelmed when needing to create or distribute iOS provisioning profiles.  This is where fastlane match shines. Fastlane tools makes easy setup for teams.  With `fastlane match` I can allow access to the profiles for easy setup on a machine.  Any developer will be able to install all the development provisioning profiles and be up and running in minutes.  All they need is the repository url and the shared key that is setup when you establish the repository for your certs and profiles.

# Setting up my environments
The following is a list of things we'll need when [getting started](https://docs.fastlane.tools/#getting-started):

#### Perform these steps in the same folder your as fastlane repository.

- **git repository:** This should be a private repository as it contains your code signing certificates and provisioning profiles.
- **iOS developer portal username:** You will need a username that has access to the teams iOS developer portal
- [Install fastlane](https://docs.fastlane.tools/#install-fastlane)
- **build.sh:** This can be downloaded from the [Cake Website](https://cakebuild.net/docs/tutorials/setting-up-a-new-project). If you use VSCode, you can use the Cake extension to generate bootstrappers, and cake files.
- **fastlane.match.cake:** I explicitly call out match in this cake file name because I intend to use more Cake.Fastlane in the future.

Fastlane itself has a set of configuration files, and lanes that can be used.  Cake.Fastlane should respect fastlanes default configuration files.  If you find out it doesn't raise an issue in the repository please!

At this point your local fastlane repository folder should look similar to the image below:
![Local Repository Folder](https://raw.githubusercontent.com/RLittlesII/rodneylittlesii/master/src/images/fastlane-folder.jpg)

# Setting up the cake script
Now we are going to give our `fastlane.match.cake` some color.  Since a cake file is just a C# file that is executable in the Cake runtime, I can use C# concenpts like global variables and local functions in my cake script.
I'll need to load the [Cake.Fastlane](https://www.nuget.org/packages/Cake.Fastlane) addin from nuget in order to use the power of Cake's addin system and refernce the relevant aliases.

In order to use the addin system in cake, we will need the following preprocessor directive at the beginning of our fastlane.match.cake file.

``` csharp
#addin nuget:?package=Cake.Fastlane&version=0.1.0
```

Next I'll define variables from the bootstrapper/command line that I might care about when setting up this cake file.  For this post I limited it to the task target, and a flag that determines which profiles I want to install.  This is something that you may want to play with.  On my build server, I may want to use `Adhoc` but locally, as a developer I may only care about the `Development` profiles.  Regardless, this is something you can tailor fit to your needs.

``` csharp
var target = Argument<string>("target", "Default");
var development = Argument<bool>("development", true);
```

Now I am going to define a set of Cake Tasks per profile.  This will allow me to only execute the profiles I want based on the development criteria, and the Cake Task depedency graph.

``` csharp
// Task
Task("Local-Development")
    .WithCriteria(() => development)
    .Does(() => 
        Match("com.contoso.demoapp.local", CertificateType.Development));

// Target
Task("Local")
    .IsDependentOn("Local-Development");
```

# Running from command line with ease
The only thing left to do is run the command line.

**Note**: You will most likely have to execute the following command on your `build.sh` file in order for the bash shell to execute it. `chmod u+x build.sh`

`./build.sh -s fastlane.match.cake -t Local`

This is a small glimpse into what fastlane tools are capable of, and how to use the Cake scripting engine for easy reusable scripts.  I have not fully integrated this into my build pipeline.  Stay tuned for further information on that.  Currently I use the Cake Script to onboard developers, and regenerate certificates when we add new devices.

Here is the full fastlane.match.cake script that I created for this post.

<script src="https://gist.github.com/RLittlesII/99357a96d54a534ac1387f43c415c59a.js"></script>