Title: Adding Cake.Recipe as my build conductor
Published: 07/23/2017
Tags:
 - Cake
 - OSS
---

- [] Attempt to use Cake.Recipe
- [] Git Hub Issue
- [] Wiring up MyGet

# A new build recipe

For those who are new to Cake.  I label Cake an automation orchestration platform.  It allows me to automate my build, and port it to another CI system with ease.  To most people, portability is not an issue.  They setup CI once and "never look at it again".  There are those of us that look at our build process, the same as we do our code, adapting to deliver value to our users.

The topic of this post is how [Cake.Recipe](https://github.com/cake-contrib/Cake.Recipe) can orchestrate your build process and the steps to wire it up.

## Before I added a new recipe

Before I added cake my build looked like this.

[build.cake](https://github.com/RLittlesII/Cake.SqlPackage/blob/develop/build.cake#L1-L228)

Not bad as far as build scripts go.  If you have ever written a build script in PowerShell you'll understand.  With a new recipe I can reduce my build script to under 10 lines of actual script!  All we'll need is the [bootstrapper](http://cakebuild.net/docs/tutorials/setting-up-a-new-project) from cake.  A cake file to call your own, and the below lines in that file.

``` csharp
#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context, 
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            title: "PROJECT_TITLE");

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context);

Build.RunDotNetCore();
```

## We have an issue
Our file structure doesn't fit the convention of what is expected.  Cake.Recipe uses C# classes to represent elements of the build process.  The task that runs the tests expects the folder our tests are in to be under the `./src` fodler.  @gep13 suggested to add build parameters to pass a globbing pattern to resolve the test path.

## Resolving our own issue
So we need to change `BuildParameters.SourceDirectoryPath + "/**/*Tests.csproj"` to something that accepts a string so we can pass our own glob pattern into the call to find tests.

Our [Github Issue](https://github.com/cake-contrib/Cake.Recipe/issues/132) stated that we wanted to add a test directory and a test pattern.  So, we'll go to the `paramters.cake` file, and add the properties we want to help our build.  Once we've completed that we'll consume them in the `testing.cake`.  The trick is to not break the current public api.  We don't want to introduce breaking changes to the public if we can avoid it.


## Getting the MyGet feed working
If you're like me, you want to test your changes before you commit them to a repository.  Doing this without a means to source packages these days is hard.  So I setup [MyGet](https://myget.org) feeds so I can pull them on build.  This way my clean script does it's work, and I don't have to make sure I always have the latest in the `./tools` folder every execution.

Getting this working with Cake.Recipe was proving to be an adventure.  The documentation states:

"https://github.com/RLittlesII/Cake.Recipe/blob/develop/docs/input/docs/usage/creating-packages.md#how-it-works"

Adding the below in AppVeyor solved the problem.  The next build execution I got my finished product in [MyGet]()
> `.\build.ps1 -Target Publish-MyGet-Packages`

https://github.com/cake-contrib/Cake.Recipe/blob/develop/Cake.Recipe/Content/parameters.cake#L376-L380
