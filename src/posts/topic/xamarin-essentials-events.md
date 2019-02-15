Title: Mocking Connectivity with Reactive Extensions and Xamarin Essentials
Drafted: 02/15/2019
Published: 03/21/2019
Tags:
    - ReactiveUI
    - ReactiveUI Events
    - Reactive Extensions
    - Xamarin Essentials
    - Xamarin Forms
---

# Xamarin Essentials

Everyone knows about the awesomeness of [Xamarin Essentials](https://github.com/xamarin/Essentials).  It is well documented on [Microsofts Documentation](https://docs.microsoft.com/en-us/Xamarin/Essentials/).  In v9.7.2 ReactiveUI released [ReactiveUI.Events.XamEssentials](https://www.nuget.org/packages/ReactiveUI.Events.XamEssentials/).

The ReactiveUI package converts the events emitted from the various Xamarin Essentials to observable sequences.  I quickly realized this was an amazing thing.  I can use this package to create abstractions for the Essentials, making for testable code.  So off I went to create a set of abstractions.  The abstraction highlighted in this post is `IConnectiviy`.

# The Abstractions

There are a lot of [blog posts](https://ryandavis.io/interfaces-for-xamarin-essentials/) around that explain why and how to abstract Xamarin.Essentials.  I started, but I wanted something that modeled asynchrony and events as observable sequences.

I settled on the following interface because I am modeling my event patterns as observable sequences.
<script src="https://gist.github.com/RLittlesII/6967844d4f9339e439c05851a1f81d24.js"></script>

# The Implementation

The implementation is straight forward.  It returns the underlying Xamarin.Essentials we care about in our app, but this practice allows me the ability to mock out the `IConnectivity` interface in my unit tests, and my mock app.  Now I can write unit tests that verify my view model responds appropriately to network connectivity changes.

How many mobile applications have you written where there is a requirement similar to the following:

> "When the user loses network connectivity, you should notify the user and cancel the current request"

Better, how many mobile applications have you used that don't notify you, and fail to provide that quality user experience?!  I can think of a few off the top of my head.

These abstractions will allow us to test the users experience without having to change the network connection!  For developers who spend more time than they would like staging tests, this should be familiar.  Why not have a repeatable way to simulate the connection that we control?

# The Mock

I wrote the implementations and realized, if I am going to reuse my abstractions and implementations, I might want to reuse my mocks.  I quickly started down the path of creating mock implementations.  The implementation details are Open Sourced, so I won't go through that here.  More importantly I will show you what we can do with the mock.

So, I have this trick I learned a few years ago.  I mock my dependencies and simulate experiences on a time loop.  I use `Observable.Interval`  to push Events through my observable sequence.  This allows me to emit regular changes to data streams so I can test the user experience.

I use a configuration singleton and build configurations to register the mocks in my DI container.  This way I can work on UI and UX without the dependency of the internet or an API.  In doing so the [Interaction](https://reactiveui.net/docs/handbook/interactions) I use to display connectivity changes to the user has the mock implementation. On an interval I define, the mock will fire an `OnNext` call allowing me to test that my user experience works as I expect.  Depending on how I setup my Reactive Extension operators, I can display a pop up that notifies the user.

Now I can simulate connectivity changes in my view while I work on making sure my users experience is what I expect.

![Connectivity Dialog](https://raw.githubusercontent.com/RLittlesII/rodneylittlesii/draft/essential-events/src/images/connectivity.gif)