Title: Not your father's build process
Drafted: 09/13/2017
Published: 01/10/2018
Tags:
    - Cake
    - Open Source
    - Continuous Delivery
---

# Cake.Recipe

I realized after I wrote about how to setup [Cake.Recipe](https://github.com/cake-contrib/Cake.Recipe), I didn't explain *what* Cake.Recipe is and *why* you might want to use it.  Cake.Recipe is a a collection of Cake tasks that is designed to help build and deploy [nuget](nuget.org/) and [chocolatey](https://chocolatey.org/) packages.  The real power of Cake.Recipe lies in the the theory behind the magic.  Applying this theory to your own use cases is a powerful way to automate Cake builds.

# Why a recipe?

Any good Cake starts with a recipe.  I still use my grandfather's butter milk pancake recipe that I learned when I was seven.  It is a repeatable, reusable set of steps to make a pancake that tastes the same every time.  Cake.Recipe is a guideline for reusing common cake script components across multiple projects.  Following the structural layout of Cake.Recipe, you can organize your scripts into concise reusable elements.  You can bootstrap loading those files and slim down your main build script.

In todays distributed architectures, you have multiple cohesive solutions, that need to be built and deployed in similar fashion.  This is the use case where Cake.Recipe shines.  It allows me to write my build tasks once and package them for reuse across multiple projects.  Using this paradigm, you can write a reusable build recipe and load it into any Cake execution.

Using the [#load](https://cakebuild.net/docs/fundamentals/preprocessor-directives#load-directive) preprocessor directive in my `build.cake` I can load any cake file into the Cake runtime.  I can package my recipe into a nupkg and load it into any build process with minimal overhead.  This allows me to write all my common components once, and use it across multiple build processes.  If the concept of maintainability and reuse is good for your code, imagine how much your build process could benefit from it?

# Recipe Ingredients
Cake exposes a few important ingredients that are important to building a recipe.

- [CakeTaskBuilder](https://github.com/cake-build/cake/blob/develop/src/Cake.Core/CakeTaskBuilder.cs)
- [ActionTask](https://github.com/cake-build/cake/blob/develop/src/Cake.Core/ActionTask.cs)

CakeTaskBuilder provides access to the task extension methods required to create a dependency chain.  I can define a task, set its depdendencies, criteria and error conditions.  Further I make that definition dynamic for each project that sources the recipe.  With a bit of thought, I can create a dynamic build process per project from a single recipe.

At this point I want to remind you that Cake is "just C#".  I can use C# static instances to pass data and decisions around without having to use method injection.  Normally I would argue that static helpers are less than ideal.  With Cake, static helpers allow me to influence build conditions at script execution without adding an inversion of control dependency.

ActionTask is the class that allows for task creation.  You can define a delegate to be executed when the Task is called in the dependency chain.  This is important because I can define my `CakeTaskBuilder<ActionTask>` and allow each script that sources my recipe to define it's own implementation.

# Script Composition
With Cake.Recipe I am using [Single Responsibility Principle](https://en.wikipedia.org/wiki/Single_responsibility_principle) for my build scripts.  I write a cake script with a specific purpose once and bootstrap all my scripts into a given Cake execution.  These scripts become single units of well defined code that do their task well.  I've still written that task once, I am still using the same recipe, but I can change the execution based on setting build parameters per script that consumes my recipe.  

The key to decoupling is how I load my scripts.  If I create a dependency web (interdependent scripts), then I will likely cause myself headache.  Instead I can write a `load.cake` and source it at the beginning of my cake execution.  This ensures that all my dependencies are loaded in one place, and that the Single Responsibility of a given script is not comprimised.  Then in my build.cake all I have to do is source my load.cake.

``` csharp
#load 'common.cake`
#load 'dotnet.cake`
#load 'nuget.cake'
#load 'chocolatey.cake'
```

# Script Execution
Decision trees can be dynamic per build execution based on the parameters.  If you have different build needs on different branches, you can set different parameters that allow the process to execute extra steps.

One thing I like to do is make sure that all integration tests are executed before I deliver to QA for testing.  I don't need my integration pipeline to slow down my green light for devs to move on to their next task.  I can actually look at the build environment, validate which branch I am executing on and only execute integration tests for that branch.  `IsDependentOn()` and `.WithCriteria()` allow me to add or remove specific `ActionTask` from a given script execution.

``` csharp
Task("Run-Integration-Tests")
.IsDependentOn("Tests")
.WithCriteria(BuildParameters.ShouldRunIntegrationTests)
```

# Short term pain, long term gain
I know your saying to yourself "That seems like a lot of work".  Yup.  It is.  A bit of up front work to be able to stand up a new project and have it built and deployed in a matter of minutes feels worth it for any operations department.

I hope you find these concepts useful.  I realized shortly after I adopted cake that I was rewriting the same tasks from project to project, I started to reorganize my scripts, and then I was introduced to Cake.Recipe.  Now I use Cake.Recipe for any library I write that results in a nuget package.  Cake.Recipe is a powerful concept that can help you create reusable build processes.