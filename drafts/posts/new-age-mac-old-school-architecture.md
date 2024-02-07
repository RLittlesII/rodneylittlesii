Title: Setup an M1 Mac for Xamarin tooling
Drafted: 04/07/2023
Published: 04/07/2023
Tags:
    - ARM
    - dotnet
    - macOS
    - Jet Brains
    - Rider
    - Xamarin
---

## TLDR
on M1 mac you HAVE to have an x64, but there is no arm x64 of 3.1 so you have to set the executable path manually in your IDE.

# Yeah ... why do a Xamarin post? who does that with MAUI out?

The guy who has to feed himself.

# YaY new Mac, boo old dotnet
If you are like me and needed a new mac for iOS development and the best option was an ARM based Mac, you likely didn't read the fine print.  I know there is an architecture difference between the x64 intel and the ARM 64 chips.  What I didn't understand at the time, running any mono based compilation would require some setup.  The remainder of this post will attempt to explain a few things.

- Why some versions of dotnet require setup for ARM chips
- Why you have to select the correct version of msbuild
- Why mono has anything to do with this
- Why "just upgrade" isn't an option

# x64 vs ARM

- netcore 3.1
- greater than net 5.0 is official ARM support

# My new fast ARM, no dotnet arm support till net x.x


# Mono runtime concerns
6.12 vs 6.13

[GitHub Issue](https://github.com/mono/mono/issues/20250) for where the current release information is.  There are unreleased mono bits that are required for some of the new compiler features.  If you need those, you can find them here

6.13 is available [here](https://www.mono-project.com/download/nightly/)

# Tools, Build and Execution

This screen in Jet Brains Rider has become my most viewed page in all the Preferences!

![Tools, Build and Execution](../../src/images/tools.build.execution.m1.png)