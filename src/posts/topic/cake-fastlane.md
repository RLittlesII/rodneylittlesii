Title: Introducing Cake.Fastlane
Drafted: 12/13/2017
Published: 01/06/2018
Tags:
    - Cake
    - Open Source
    - iOS
---

# Fastlane tools
Do you develop iOS applications?  Do you dread every request you get to add new devices to a provisioning profile?  Or regenerating APNS certificates?  Have you heard of [fastlane tools](https://fastlane.tools/)?  I had not.  I was wasting hours of development time, or time I could fight with Xamarin, to add one off devices for testing.  Then, a developer told me of a better way.  I found fastlane tools! These tools were created to save the development community hours upon hours of time wasted.  These tools have saved millions of developer man hours, and I can't believe I didn't find out about them sooner.

# Cake all the things
Because I love Cake, I like to port all the command line tools I use for easier consumption to Cake's domain.  This makes it easier for me to automate, and interact with those tools in Cake scripts.  I could very easily use Cake to start a process, pass parameters and not bother with the reuse the abstraction the DSL provides. To me one of the the sweet part of consuming Cake is playing with the batter in the DSL.

# Cake and Fastlane, together
I present to the reader, [Cake.Fastlane](https://www.nuget.org/packages/Cake.Fastlane/)!  Cake.Fastlane is nothing more than a Cakified (word?) wrapper around the oh so amazing fastlane tools.  The current v0.1.0 offering only implements [fastlane match](https://docs.fastlane.tools/actions/match/).  Be sure to stay tuned for future releases as the addin will eventually provide fastlane deliver, and other tools of the fastlane echo sytem.

# How to consume Cake.Fastlane
Using Cake.Fastlane aliases is slightly different than most [Cake Aliases](https://www.cakebuild.net/docs/fundamentals/aliases).  I did not want `FastlaneMatch()` and `FastlaneDeliver()` because I thought this felt a bit disjointed.  Lucky for me Cake provides a convenient way to handle this.  I elected to use `CakePropertyAlias` so all the fastlane method aliases could be grouped together in a fluent manner that allows access to the Cake Context.  Using the Fastlane property makes extending the fastlane tools look more like `Fastlane.Match()` and `Fastlane.Deliver()`.  This felt much more like a suite of tools that extended from a single property, as opposed to a bunch of related aliases.

This example from the xml documentation should get you up and running using Cake.Fastlane

```
    Fastlane.Match(config =>
    {
        config.CertificateType = CertificateType.Development;
        config.AppIdentifier = "com.fastlane.cake";
        config.ForceForNewDevices = true;
    });
```

# Things to do
I have recently updated all the xml documentation for Match, but need to submit a pull request to the Cake Website for documentation inclusion. All that is slated for the 0.2.0 version of the addin.  Please stay tuned for more Cake Fastlane fun!!!