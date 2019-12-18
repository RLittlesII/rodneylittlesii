Title: Using Microsoft Build Central Package Versions for Xamarin
Drafted: 12/18/2019
Published: 01/09/2020
Tags:
    - MSBuild
    - Xamarin
---

## NuGet Version Shenanigans
Have you ever worked on a project where any given developer adds nuget packages references.  Have you ever audited package versions across a solution only to find your issue is version mismatches?  Have you ever had two different projects depend on two different versions of the same dependency?!  Have you sourced a package only to receive an error because another package relies on a different version of the same dependency?  If you answered yes to any of the questions, you're in luck!  Today we are going to see how [Central Package Versions](https://github.com/microsoft/MSBuildSdks/tree/master/src/CentralPackageVersions) can help alleviate the issues of package dependency purgatory.  This concept isn't new, there is now an MSBuild Sdk that provides a central way to manage the concern.

## Build props and targets
Recently I started using `directory.build.props` and `directory.build.targets` to provide global functionality to projects of similar types in a folder.  This is something like adding roslyn analyzers to a projects in a code base, but explicitly not using it for unit test projects.  It can also be a way to provide simliar package metadata to a mutli-project, multi-package solution.  Instead of duplicating the metadata in each .csproj in the solution, you can provide the package details like `RepositoryUrl` or `License` to all .csproj files in the solution.

For a while this worked fine, but there was still no standard way to handle things like package version overrides, or enforcement of package versioning.  It didn't matter, it was better than how things were.  MSBuild Sdks provides an explicit Sdk for Centralizing Package Versions, and enforcing consistent package consumption for a repository.  When I first heard about it, I was excited!  As with most things, I become less excited when I find out that it doesn't work, or isn't supported for Xamarin.  This was no exception.  I perused the docs (scanned quickly) and was convinced it didn't work for Xamarin solutions.  I was wrong.

## Central Package Version
If you use git, then drop a `packages.props` file in the root directory of your repository.  This `packages.props` file is going to be the single source of truth.  That means you can no longer right click and add a nuget package to a single project, it has to be added here.  The packages props will maintain a global version reference.

I prefer to put a common props and target file at the root of the repository.  You don't have to, you can add the following to a directory target, or on each individual .csproj file.  Regardless of where you place it, the following xml will tell the MSBuild engine to use the Central Package Version Sdk.

```xml
<Project>
    <Sdk Name="Microsoft.Build.CentralPackageVersions" Version="2.0.52" />
</Project>
```

### Package References Types

In your `packages.props` you can now define a few specific reference types.

```xml
<GlobalPackageReference Include="Microsoft.SourceLink.GitHub" Version="1.0.0" />
```

- [Global Package References](https://github.com/microsoft/MSBuildSdks/tree/master/src/CentralPackageVersions#global-package-references) - This reference type should be used on any dependency that is used repository wide.  It is as the name suggests a global inclusion for all projects.

```xml
<PackageReference Include="System.Reactive" Version="4.*" />
```
- Include Package References - This reference type includes the package reference in all projects.

```xml
<PackageReference Update="ReactiveUI" Version="11.*" />
```
- Update Package References - This reference type updates the package reference from the metadata.


## But Why?

At this point, it would have registered that I said, no right click and adding nugets.  Yes, I know some readers will find this too complicated.  The tedious nature of having to update XML is just too much for some to bother with.  Remember one of the benefits of this Sdk is to make maintaining a **solution** easier.  So there is a trade off.  You trade a simple GUI user experience, for not having to dig through every single project just to find a single project has the wrong dependency version.

### Benefits

- Forcing a consistent dependency version
- Providing an easier way to source packages used by multiple projects
- Less package version audits
- Fewer problems updating package references
- Global package avilability

### Drawbacks

- Developers on a team can no longer source individual files using the nuget explorer
    - That's right, you have to add XML directly to the packages.props and know what version you want!
- More confusing to someone where package dependency versions are kept
- You have to explicitly request and override of a version

I won't bore you with showing you how to setup the `packages.props` instead here is a link to a Xamarin.Forms project using it, Happy Package Management!

https://github.com/RLittlesII/demo/tree/master/msbuild/CentralPackageVersion