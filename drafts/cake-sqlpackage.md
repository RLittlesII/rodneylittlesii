Title: Adding sql package to Cake's ingridients
Published: 04/26/2016
Tags:
    - Cake
    - DevOps
    - OSS
---

# Cake Addin
___

Cake has a rich ecosystem of [Addins](http://cakebuild.net/addins/) that allow extensibility of the [DSL](https://en.wikipedia.org/wiki/Domain-specific_language) for various tools to be consumed in a native fashion to [Cake](http://cakebuild.net).  Addins are easy to write, and are generally a wrapper around a tool that makes it easier to consume in a cake script.

___


## How to get started

- New Project class library that targets a version that can consume [Cake.Core](https://www.nuget.org/packages/Cake.Core/)
- Aliases, static methods executable inside the Cake Context
- Runner which will do the work for the aliases
- A Cake script to build your Addin
- A nuspec file to share your Addin with the world!



### Defining the specification
[SqlPackage.exe](https://msdn.microsoft.com/en-us/library/hh550080(v=vs.103).aspx)

### Finding a nuget package to deliver dependencies
[Microsoft.Data.Tools.Msbuild](nuget:https://www.nuget.org/api/v2?package=Microsoft.Data.Tools.Msbuild) provides us a means to use SqlPackage.exe from a build server without any preinstall of the executable.

### The heavy lifting of Cake.Core

### Package the Nuget

 - Package
 - Publish MyGet
 - Use in Build


### The end result: [Cake.SqlPackage](https://www.nuget.org/packages/Cake.SqlPackage/)