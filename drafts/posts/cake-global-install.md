Title: Building with Cake at the Airport
Draft: 09/27/2017
Published: 09/17/2017
Tags:
 - Cake
---

# Cake.exe in every project folder
If you like your cake, and eat it to, you probably have a lot of Cake build artifacts hanging around.  I have artifacts in each folder that uses Cake.  I always thought "Meh, storage is cheap", so I never worried about cleaning up my crumbs.  When I read that [Cake v0.22.0](https://cakebuild.net/blog/2017/09/cake-v0.22.0-released) resolved [Github Issue 753](https://github.com/cake-build/cake/issues/753) I reconsidered my stance.  I decided having a global cache of versioned dependencies was better than scattered versioned dependencies.  Secretly I am hoping a global cache may get me one step closer to local offline build.

# One Cake to rule them all
Cake has several configuration options for setting up a global folder.

- Environment variables
- .ini file
- Command line argument

Command line doesn't feel scalable and .ini files worry me.  I consume a lot of Cake and I want each of my projects to source their dependencies from one folder.  I don't want to make configuration changes per project, some of which I don't own.  I've decided to go with environment variables because they are consistent across platform and accessible from the command line.

#### Powershell
<script src="https://gist.github.com/RLittlesII/2c32b2dbb8d82d0e35f06cde178463fd.js"></script>

#### Bash Profile
<script src="https://gist.github.com/RLittlesII/e73b0519b4ef38e70c8b7fda8bcb2f40.js"></script>

# Modify the bootstrap script
If you're lazy like me, you probably just download the stock bootstrapper from [Cake](https://cakebuild.net/docs/tutorials/setting-up-a-new-project).  Today, we have to extend the bootstrap script and make it aware of our global configuration.

``` powershell
$ToolPath = [Environment]::GetEnvironmentVariable("CAKE_TOOLS_PATH", "User")

if($ToolPath -eq $null)
{
    $ToolPath = Join-Path $PSScriptRoot "tools"
}
```