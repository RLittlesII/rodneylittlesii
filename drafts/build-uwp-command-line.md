Title: Building UWP from the command line
Published: 07/24/2017
Tags:
    - Cake
    - DevOps
    - UWP
---

# The Players

Below are the command line utilities that one would use to create a UWP app that can be Sideloaded.

- MSBuild - Build the application with MSBuild
- [MakeAppx](https://docs.microsoft.com/en-us/windows/uwp/packaging/create-app-package-with-makeappx-tool) - Utility to create app package and app bundles.
- [Signing Tool](https://docs.microsoft.com/en-us/windows/uwp/packaging/sign-app-package-using-signtool) - Sign the app package or bundle with a certificate.


## MSBuild to the rescue
---

[Microsoft Documentation](https://docs.microsoft.com/en-us/windows/uwp/packaging/auto-build-package-uwp-apps) shows us that you can send the following MSBuild arguments.

``` powershell
/p:AppxPackageDir="$(Build.ArtifactStagingDirectory)\AppxPackages\\"
/p:UapAppxPackageBuildMode=StoreUpload
/p:AppxBundlePlatforms="$(Build.BuildPlatform)"
/p:AppxBundle=Always
```

The above command should make it so that we don't have to explicitly call `MakeAppx.exe`.


## Encountering Errors

```
MakeAppx : error : Error info: error 80080204: All app package manifests in a bundle must declare the same values under the XPath *[local-name()='Package']/*[local-name()='Dependencies'].
```

I followed the instructions from the docs, what gives?!

A quick google search turns up an existing developer who has encountered my issue.

[robgruen's WebLog](https://blogs.msdn.microsoft.com/robgruen/2016/04/13/makeappx-error-error-info-error-80080204-all-app-package-manifests-in-a-bundle-must-declare-the-same-values-under-the-xpath-local-namepackagelocal-namedependencies/)

`code .` and we begin digging through the .sln file to remove `Any CPU`

Success.  Now when I right click and Store => Create App Package it builds to completion.  The install still fails.

![UWP Installtion Error](..\src\images\UWP-Installation-Failed.png)