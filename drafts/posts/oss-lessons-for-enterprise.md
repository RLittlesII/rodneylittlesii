Title: A few lessons Enterprise can take from Open Source
Drafted: 08/13/2022
Published: 08/13/2022
Tags:
    - C#
    - .NET
    - dotnet
    - Enterprise
    - Open Source
---

## Disclaimer
My only experience with Open Source Software (OSS) is in the dotnet community.  I understand that different communities have differeent concerns and this may not be applicable everywhere.

## The spirit of software
Before you say it.  No this is not about vision quests or spirit animals.  This is about understanding what is being requested, vs doing what we are told.  If a user story comes to you and has five acceptance, and you look at it an wonder, "is this really going to work the way the business wants it to work" then you understand the spirit of the request.  If you look at the five acceptance and you ensure they are covered without considering edges, corners or potential missed requirements; your doing what you are told.  Yes, yes.  I know that this is extremely gray area in a world that is so black and white.  Lets consider the real world for a moment.  How much does it cost a company in additional man hours to send a feature to QA, just to have it returned to the developer because it doesn't work "as expected"?  Then my favorite game of pin the blame on the developer starts.  We go around with QA and the business for a few hours, or days, all to find out we missed a spot!  At this point, it honestly doesn't matter who missed it, the developer has to fix it.  Most of this could be avoided by a conversation before the developer considers the code complete and ships it.  Who has time for that?  I satisfied the five acceptance, I am done!

As developers we forget, our job isn't to punch a clock and close tickets.  Our profession exists to solve real problems for *real* consumers.  If we get to create something fun and exciting and learn something during the process, *bonus*!  Open Source understands this concept, enterprise sometimes misses the mark.  So the spirit of the software is that place between acceptance criteria and how a real user would use the application, and the developer is the first human to get to ask the question, "will the implementation satisfy the acceptance *and* make the consumer experience enjoyable.

## Developer Ownership is powerful
In OSS, developers are running the show.  They put their hard work, time and effort into every line of code they write.  They own it, and as a result they are usually very concious of it and want it to work.  In Open Source we have metrics that help us judge the health of a project.

- Open Issues
- Release Cadence
- Number of Package uses
- Age of project

In enterprise, the enterprise owns the software.  Developers own work items.  These work items need to be moved to a done state as soon as possible no matter the cost to the maintainability, scalability, or compatability.  Open Source developers generally care about the spirit of the software we talked about earlier.  To them it isn't code, it's a life they have to make sure is healthy.
- When devs own it they care about keeping it.
    - Tragedy of the commons

## Quality is an important metric
- OSS is dependend on by many developers to build their applications on top of
- Without a quality bar nobody would consume
- Trust is more important than timeline
    - once you lose the consumers trust, its difficult to regain

## Failing fast is less risky than failing slow
- Point releases allow confident developers to find issues in your software
- Main line development
    - ci
    - automated unit tests
- Developers finding bugs is better than QA finding them
    - Developers give better detail about the crux of the issue
    - Developers 

## Maintainability as a service
- Pull Requests add value
    - Ensures design decisions are respected
    - Ensures quality is met
        - Code coverage
        - Automated testing
- Processes
    - Issues to identify bugs
        - Templates
        - If templates aren't filled out correctly issues are closed
    - Issues with reproduction of the problem
        - proper reproduction of an issue is important
- Support

## Abuse of maintainers is not tolerated

# Documentation
- Docs
- Issues
- Pull Requests
- RFC's

## Branching, Releasing, Versioning
- Understanding the world of breaking changes
- Know what it is to have breaking API changes
- Understanding the concepts behind semver
- Software readiness

## Don't reinvent the wheel

## Invest in the software