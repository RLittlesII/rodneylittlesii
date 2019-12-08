Title: Contributing to Open Source is more than just code
Drafted: 12/06/2019
Published: 12/07/2019
Tags:
    - Open Source
---

## Want to contribute to Open Source Software?

Getting involved in open source is more than just writing code. I have had this same conversation with several developers over the last week.  So you know what that means, **BLOG POST**!

There seems to be this idea that the only way to contribute to Open Source Software (OSS) is by submitting massive pull requests with thousands of lines of improved code.  I am here to tell you, while those pull requests are appreciated, they are not the only contributions that matter.  There is more to helping OSS initiatives than slamming code!

<blockquote class="twitter-tweet">
    <p lang="en" dir="ltr">Number of lines of code written is not a measure of your value to a project.</p>&mdash; Scott Hanselman (@shanselman) <a href="https://twitter.com/shanselman/status/1116955928660566016?ref_src=twsrc%5Etfw">April 13, 2019</a>
</blockquote>
<script async src="https://platform.twitter.com/widgets.js" charset="utf-8"></script>
<br/>

## What is a contribution?

#### A conversation
So much can be exposed through conversation.  Often we make tools to solve problems.  As a maintainer, the problems we need to solve may not fit your use case (although generally, they do).  Have a conversation about an implementation detail you don't understand or agree with.  The key to this is tone.  People generally don't take well to you attacking their decisions.  If you've written code long enough you will know that you make concessions along the way, OSS is no different.  Be respectful, state your case, let the conversation play out.  Sometimes that conversation can help drive change, but don't be discouraged when it doesn't.  80% of the world may not need what you need and that's okay!

#### Asking relevant questions
I preface this with `relevant`.  I understand that sometimes we need an expert opinion.  Asking MSBuild questions in the build automation tools chat seems great, and they will most likely help you find the MSBuild documentation you seemingly missed.  Questions that challenge the framework boundaries, inspire new features, or expose potentially restrictive architecture are the best kinds of questions.  Again, tone matters.  If you're aggressive behind your computer, you most likely won't be received well.  So be mindful to ask in a spirit of helping rather than anger or frustration.

#### Answering questions
Stack overflow gets questions all the time. Set up a rule to monitor specific words and jump in and answer questions.  A lot of projects have chat channels these days.  Jump in, join the conversation.  Your knowledge of the framework can help countless other developers.

#### Identifying and Reporting issues
I appreciate this breed of contributor.  They find the holes.  They are consumers, they care enough to do some research and report an issue.  I think the absolute best way to report issues is with an issue repository.  This is a public repository where the maintainer can go download your code and reproduce the problem.  You take out the concern that the steps you provided aren't overlooked. If I don't have to set up your reproduction sometimes it is easier to diagnose the problem. If you can provide failing unit tests, that is another good step.  Helping regress issues is great.  Finding the exact version of NuGet dependency stew that introduced a bug will save time the maintainers can use to triage the issue, rather than identifying it.  Then, if you are able, and fix the problem and submit a PR everyone wins!

#### Requesting features
We all write code, and we all know how to skin the proverbial cat.  What we don't always know is how the maintainer envisions the direction of the project and codebase.  This is always the hard part of OSS.  It is easier for a contributor to just slam code in a PR than it is to talk about it.  Some repositories have contribution guidelines that will ask you to submit a feature request first. This helps flush out any design issues, questions on whether the feature has value, and allows for the potential contribution to be readily accepted when completed.

#### Promoting the framework
If you're happy and you know it write some code!  Jokes aside, if you have a good command over the framework, write some samples.  Do a blog series.  Stream how you incorporate the framework into your codebase.  Share your knowledge with the world!  When I started with ReactiveUI, I got excited about the possibilities because I watched videos of nerds on [Channel 9](https://channel9.msdn.com/shows/Going+Deep/E2E-Erik-Meijer-and-Wes-Dyer-Reactive-Framework-Rx-Under-the-Hood-1-of-2/) be so excited about math I didn't understand, I knew it had to be fun!  Take your appreciation of how the framework makes your life better and share it with the development community.  This is a great way to bring new consumers and contribute to the success of an OSS initiative.

#### Test beta packages
If you are brave enough, subscribe to the beta channel!

Report issues with beta packages to help keep the bugs from reaching the rest of the consumers.  This is hard for most people to do.  Depending on the project, the release cadence is healthy enough where this isn't needed.  If there is an opportunity to help in this capacity, you would be performing a great service.  Unit tests are great, but there is nothing like pulling down a package and integrating it into a codebase to determine if it works as intended.

## Contribute

After reading this I hope you understand how you can contribute to the frameworks you consume.  While maintainers want your code, they will be happy with your investment in the initiative.  Working together as a community to make the software we write better!