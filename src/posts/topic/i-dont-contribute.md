Title: I dont contribute to open source
Drafted: 04/18/2018
Published: 06/21/2018
Tags:
 - Open Source
---

# Free Software?!
Who doesn't like free stuff?!  I like free, especially when it comes to software.  I can build my core business functionality on the shoulders of giants, and get to market quicker.  Give us, us free!

The truth is free software, isn't really free.  You know that as a developer.  How much is this going to cost in man hours?  Hours have rates.  Rates require payment.  And yet there is an entire community of developers who provide their services, outside of their day job to bring tools consumed by the .NET Community.

If you have ever used xUnit, NSubstitute, NewtonSoft JSON or any package sourced from [NuGet.org](https://www.nuget.org/) then you have encountered these Open Source Software (OSS) developers and their work in some fashion.  Most developers don't think twice about it.  They go to NuGet, consume the packages, if there is a bug they complain about it, work around it, and go on about their lives.

Yet, there is a community of people who rally behind these freebies and do their best to make robust tools that will help the every day developer be more productive.  Their time isn't free.  Their hourly rate isn't any less than the next developer.  They just care about solving the problems they face consistently, and making that solution available to help their fellow developer.

These people are true [Bridge Builders](https://www.poetryfoundation.org/poems/52702/the-bridge-builder)!  The Robin Hood's of the development world.  They take from their time, to provide the tools so us poor developers can focus on business problems, instead of solving the non business problems with our businesses time.  We don't have to write that code, there is a library that does it.  These libraries are the foundation of what we build our businesses future on, because generally these libraries are not our businesses core business.  They are responsible for a lot of the boilerplate code that we don't have to manage anymore.  They support it so we don't have to.

Generally, all these developers ask for in payment is a thank you, an issue if you encounter a bug, or a pull request fixing a bug.  They do the work that helps our industry, for no monetary gain.  This community is seeking to expand, so that the tools developers depend on don't stagnate and become depricated.  The community doesn't seem to know how to get more people involved, and is actively searching for ways to bring in fresh minds to rally behind this cause.  But there are reasons developers don't contribute.  There are reasons I didn't contribute.

# Think Global

That's right, this means that every opinonated developer around the world can come to a repository and tell you how they could have written it better.  They can tell you, you should have caught that bug, they can belittle your efforts to help advance our industry.  On the other side of that coin, every developer in the world can help, provide guidance, and promote your efforts.  There is an entire globe of developers writing software.

> Because this is a global effort, my peers from around the world can see that github issue where I ask a [stupid question](https://github.com/cake-contrib/Cake.Slack/issues/2) and make me feel tiny for not having a bigger brain!

As you can see in the issue above, once I got past my initial not knowing what the hell I was doing, the repository owner was glad to help.  This was my first contribution to an Open Source Project and I have been helping when and where I can ever since.  Not everyone has to create the latest new hot framework.  Sometimes, adding a feature or fixing an annoying bug makes the community a better place.  There are no stupid questions, save the ones unasked!  Ask a question, get some guidance, it may help the software grow!

> Candidly I'm not that smart.  I get by day to day, but I am not a genius like some of these big named developers in the industry.  So I am just going to leave it for people who are smarter than me to handle the difficult problems.

There are many reasons this line of thinking is not productive.  I'll touch on a few.  The only way you grow is to learn.  Now, if you're fine being on .NET 3.5 for the rest of your career, then you're right OSS may not be for you.  If you're looking to understand how the industry is shaping, what the latest trends are, and how folks are architecting code these days, there is no better place for a developer to get smart.  By getting involved you will learn.  By asking questions you will obtain answers.  By contributing you will gain experience. 

"A journey of a thousand miles starts with the first step" --Lao Tzu

# System internals are hard

Learning the internals of a system can be taxing.  So many developers have all the business knowledge in their head and have to walk around with subsystems memorized just to get through the day.  Getting involved in OSS changed that narative for me.  I don't have to own entire subsystems.  My unit tests hold the knowledge of the subsystem (which unit testing is one of the things you will likely learn from an OSS project).  You can bring these concepts to your day job.  When someone asks "How does it work?", you can reply "Let's consult the unit tests."

> I don't have time to be an expert

I workout daily, I have an active social life and I am trying to open a Kung Fu school.  I partition my time to allow me to be as productive as possible.  Sometimes I can't help the community, but when I can I make it a point to do so. The frameworks I consume, I contribute to.  So there is a benefit of knowing the internals.  Knowing what my software is built on, at the code level, helps me make better technical decisions.  I don't have to be an expert, I just need to know enough to feel confident in the framework, and trouble shoot it when I am not.

> I have my own backlog and bug list to fix, I don't want to help someone else clean theirs.  Writing an application is easier than maintaining one, so why would I want to get my hands dirty helping someone keep their code bug free?

This is a perfectly valid line of thinking.  I used to think this way.  One day I realized, I depend on OSS to deliver business value.  Not giving back hinders my ability to deliver that value.  Can you count the number of NuGet libraries your applications depend on?  For each one of those there is a developer contributing his time to allow you to benefit.  Why not help by fixing an issue if you encounter it?  Fixing that bug may fix your bug, and if it's fixed once, everyone benefits.  Not to mention that extra code you wrote to work around the issue will have to be backed out at some point, right?

> I don't know how to do this.  I would love to help, but I have **NO** idea how to implement that feature!

Ask!  Someone will help you, because helping you helps them.  Maybe you are a one time contributor.  Maybe one pull request leads to a few contributions a year.  Multiply that by N developers and the software stays a float.  There have been plenty of issues I have started and needed guidance on.  Few people in OSS expect you to be brilliant, everyone expects you to be there to learn.  Jump in if your willing.  Find a project you like, you'll be surprised at what you learn.

> I struggle enough with code throughout the day.  The last thing I want to do is struggle when I get home.

Practice makes perfect.  When we are coding for our lives, we aren't practicing we are trying to survive.  I found, the more code I wrote, the easier it became.  The more peer reviews I had, the easier it was for me to architect clean code.  I discovered that the time I spent looking at someone elses code, actually made my code cleaner!  I learned the proper way to handle `iDisposable`, not just by browsing stack overflow, by implementing a feature for OSS.  Now that code is being used in a project that was able to benefit from my learning a new tool. 

> I just want to netflix and chill

Okay.  Enjoy yourself.  We all need to relax some days.  Still, if you don't work on professional growth, making career moves in the future will be hard.  You will be that developer keeping the duct tape fresh on a VB6 app that nobody knows how it works but you.  There is merrit to this.  Not everyone cares about being up to date on the latest technology.  Some folks just want a pay check and that's it.  The web has changed countless times in the years since I started C#, it will change more.  Get ahead of it.  There is the bleeding edge and the blunt edge of technology.  Both are hazardous, both hurt.  The amount of pain is relative to the business value, and your pay check could be tied to how you deliver said value.

# You may be right, but I was wrong

I was told by a friend "Find a project you like, get involved".  [My first pull request](https://github.com/cake-contrib/Cake.Slack/pull/1) was to Cake.Slack.  I fixed a typo I was seeing in my build output because it was bothering me everytime I saw it.  That pull request has sent me down the path of contributing to [Cake Build](https://cakebuild.net) for almost two years. I hang out in chat, answer an occasional question, author a few addins and document anything I can.

Has it been pain free? Mostly.  I have had a few negative experiences, some learning moments, some teaching moments.  I don't give back as much as I would like.  I still get nervous before I hit post on every issue I add my three cents to.  I enjoy what I am able to give, even if it's not as much as others.

The .NET Open Source Community needs developers.  Some from the community have taken time to teach me lessons that have helped me grow.  What I found is I didn't contribute to OSS, the OSS has contributed to me.  I was a mediocre developer when I came to the OSS community, but I saw other developers solve problems that baffled me at times.  I wanted to feel like I could keep up with my peers.  I honestly still don't know if I can, but now at least I know when I am too far stumped, I can call on experiences I have had.  I can look through a library where a pattern I am unfamiliar with is implemented.  I can ask someone who I have interfaced with and get a second, or third opinion.

So I leave you with this thought.  You shouldn't contribute to OSS, you should contribute to yourself.  If the community benefits from your contribution, then you, the community and the industry are better for it.