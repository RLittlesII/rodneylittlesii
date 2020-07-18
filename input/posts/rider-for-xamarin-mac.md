Title: JetBrains Rider for Xamarin development on Mac
Drafted: 09/14/2017
Published: 10/03/2017
Edited: 08/15/2019
Tags:
    - Jet Brains
    - Resharper
    - Visual Studio
    - Xamarin
    - OSX
---

# Long ago in an IDE far far away...
I have been a .NET developer for a long while now.  I remember Java during the solaris days when you had to write in notepad and compile command line.  Sparing you the rest of this trip down memory lane, I have been writing code since before IDE's where a mandatory requirement to write code.  As such, I have used many IDE's in my time, and I have been spoiled by Visual Studio, having no need to look further for an IDE.  Visual Studio has its good and bad features, which I won't enumerate here,  but all in all it's a powerful IDE that helps with producivity.

Recently, I have adopted [Visual Studio Code](https://code.visualstudio.com/) as my text editor of choice (because it is IDE light, and [OmniSharp](http://www.omnisharp.net/) provides a nice intellisense experience).  With the advent of [Roslyn](https://github.com/dotnet/roslyn), intellisense has become a game of one-upmanship.  Everyone is trying to provide a better developer experience and a lot of meta programming for tooling has made developing wih C# a fun experience.

The most recent IDE to come across my desk is [JetBrains Rider](https://www.jetbrains.com/rider/).  I was skeptical, until someone in the Cake gitter channel said it was the most productive cross platform IDE he has used.  I made a resolution with .NET Core to start doing more development on my Mac, being a Xamarin developer, it's where I spend the majority of my time.  The remainder of this post is about my experience with and the contrast betwwen Visual Studio 4 Mac and Jet Brains Rider.  


> **DISCLAIMER: I am new to IntelliJ IDE's and some of this assessment may be my ignorance.**

# Evaluation being new to IntelliJ
First assesment of Jet Brains Rider based on my expectations from Visual Studio is favorable.  It has a lot of nice features, and more importantly the features that I have come to expect are present.
- Pros
    - Resharper on Mac
    - Resharper on Mac (This is not a typo, it deserves to be here twice because it's THAT important)
    - Can pin tabs
    - Code clean up
    - Unit Test explorer
    - Debugging shows variable values
- Cons
    - No solution explorer sticking to current document
    - Picking build configuration is a pain, I have A LOT of build configurations which are not sorted in the drop down
    - Dragging and dropping window panes (debuging, unit test) was not possible
    - No code lense

# Evaluation for Xamarin
- Pros
    - Debugging in Emulator just works, was up and debugging an existing Xamarin.Forms project in minutes!
- Cons
    - No new ContentPage control, have to use WPF user control, and hack around it
    - Does not default to the plugged in device
    - Could not get debugging working on iOS, Android was fine
    - Forces me to choose a profile when I have Developer/Automatic
    - Does not let you edit values directly, instead prompts you based on it's decision logic
    - Forces me to select the emulator every time I start debugging, even though I have a default configuration with one selected.

# Jet Brains Rider
Barring some of the things I listed. Yes the list is long, but most of them are minor things I can work around for now. Rider is a nice IDE.  I am still partial to Visual Studio as an IDE, primarily because I have been coding in Visual Studio for so long and change is hard.  I can see making Jet Brains Rider my default IDE for Xamarin development on Mac once some of the more glaring holes get fixed.  As it stands, for developing .NET on Mac, Jet Brains Rider blows VS Mac out of the water.

## Edit: Rider Improvements as of 08/15/19
I have been using Rider as my primary IDE since the writing of this post.  Some of the cons have been fixed.  The Rider team seems to be working hard to make the product better.

Xamarin improvements include:
- iOS debugging has improved dramatically
- The default emulator selection issues have been resolved
- Xamarin Forms XAML `ContentPage` has become a first class citizen
- Build configurations have become manageable, sorted by default
- Code Lense is implemented
- Xamarin.Mac support
