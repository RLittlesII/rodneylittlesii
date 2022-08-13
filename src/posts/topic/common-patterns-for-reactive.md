Title: Common problems using Reactive Programming in C#
Drafted: 10/22/2020
Published: 01/27/2021
Tags:
    - C#
    - .NET
    - dotnet
    - Reactive Extensions
---

## Rx is Magic

There are a lot of misconceptions about declarative programming.  Even more so around [Reactive Extensions](http://reactivex.io/), which is a cross-platform specification for implementing the [observer pattern](https://en.wikipedia.org/wiki/Observer_pattern).  This is a well-established pattern for software development that has existed longer than I have been developing software.  Microsoft even spawned Reactive Extensions in 2009, yet so few C# developers seem to understand what it is or want to use it.  This usually happens when we don't understand how to solve problems with a declarative paradigm, which is really about declaring behavior based on state transitions.  So let's look at common problems and patterns you might encounter with Reactive Extensions.

### A few notes before we get started
Reactive Extensions is not [React](https://reactjs.org/).  It is not [Reactive Native](https://reactnative.dev/).

Reactive Extensions is a cross-platform specification for modeling asynchronous events.  [System.Reactive](https://github.com/dotnet/reactive) provides a clean discoverable API for modeling .NET Events and asynchronous programming.  They allow the use of LINQ operators you know and love paired with new ones to handle pushed based collections with ease.  .NET Events are implemented as asynchronous notifications. They happen, any handler can respond by invoking an action, then it ceases to exist.  The notification is a stateless autonomous signal.

## Observables are streams
With any new technology, there are learning curves, Rx is no different.  Inverting your thinking from a pull-based collection with a finite amount of items to a push-based list that will emit values over time to completion.  This is a lot different than grabbing a finite set from a database but still very much the same.  The following are some common issues that I ran into when I first started using observables.

### My observable isn't doing anything

An observable is a pipeline with a callback that gets invoked when it is subscribed to.  Every now and again, I will wire up an elegant observable pipeline, slam F5 only to see no action!  No value ticks, nothing actually happens.  After a few moments of frustration, I will realize there is no `Subscribe()` at the end of my pipeline.  In Rx, adding a `Subscribe()` is the equivalent of invoking the action of the pipeline.  It's worth noting that every subscription generates a new handler.  This makes paying attention to disposal important.  `Subscribe()` returns an `IDisposable`, which makes it very convenient for hooking into garbage collection.  So if the observable doesn't fire, verify that the pipeline has an observer.  Observables are asynchronous and can be awaited using the `await` keyword.

**Pro Tip:** Sometimes you will get a red squiggly that tells you it can't find `Subscribe()`.  Import the namespace and it will go away.

### I want to batch events to get all the values

.NET events are raised and if they are not handled you lose them.  This means you have to respond to every event that happens or lose your opportunity forever.  This becomes cumbersome if you need to track the existence of more than a single event at a time if the entire payload you want to be spread across multiple events.  You may want to batch the notifications or get all of the change notifications after a period of time.  There are several operators provided that allow you to defer execution and hold onto a list of changes.

#### Throttle

.NET Events happen and then they are gone.  Observables allow us to us a Throttle operator (Debounce) which allows you to delay change notifications and get the last change after a period of time.  Where this is extremely useful is Text Entry.  You can throttle back `TextChangedEventArgs` and instead of having to respond to every single event, you only have to respond to a single event when the user has finished sending `TextChangedEventArgs`.  This allows you to reduce screen lag because the event handler (subscription) isn't being called every time a new value is pressed.  It also allows you to throttle the events on another thread which prevents blocking the user interface thread while you wait for the throttle timer to tick.

```csharp
observable
  .Throttle(TimeSpan.FromMilliSeconds(350), TaskPoolScheduler.Default)
  .Subscribe(stuff =>
  {
    // This will be the result of the changes at the end of the TimeSpan
  });
```

### I want to defer subscriptions

I actually covered this in a [previous post on `.RefCount()`](../../src/posts/topic/rx-publish-refcount.md) for the sake of this post I will a brief explanation.

#### Publish

The `.Publish()` operator returns an `IConnectableObservable` this gives you the ability to wire up all your subscriptions, and then call `.Connect()` to see them all start working.  This defers the subscription of the observables until the developer has decided it's okay for them to become active.  So you can wire up all your subscription logic to an active stream and not start getting values on that stream until you actually connect to do so.

#### Connect
The `.Connect()` operator allows you subscriptions on an `IConnectableObservable` to turn on.  Once you `.Publish()` you have to later connect or your `.Subscribe()` will do nothing!  One of the side effects of the `.Publish()` all downstream subscriptions are connecting to the same invocation.  So they are all sharing the same state.  This may or may not be intentional, so be aware that it will be the same and understand what that means in the context of your pipeline.

#### RefCount
`.RefCount()` does exactly what you would think.  It keeps a reference counter to the `IConnectableObservable` to determine how many observers there are.  Once the number of observers is zero, `.RefCount()` will release the subscription and not do anything again until a new observer subscribes.  This is basically an "auto connection" operator.  It will dispose of the when there are no observers left in its reference and then invoke again when there are observers again.  This is good for things like network calls, or i/o calls.  If you have multiple observers you want to subscribe to the same return, you wouldn't want to fire that code off **every** time.  That would incur additional i/o calls.  So instead, you can `.Publish().RefCount()` and as long as there is a single observer, everyone else observing that pipeline will not make a new i/o call.

### I need similar parts of the observable to manage different state
Observables are composable!  Just like any LINQ statement on an `IEnumerable` you can branch an `IObservable`.  The main difference here is every `.Subscribe` will invoke a different handler.  So at any point, you can take an event notification on an observable pipeline and compose a new pipeline.

```
var valueChanged = this.WhenAnyValue(x => x.Property);

valueChanged
  .ToProperty(this, nameof(NewProperty), out _newProperty);


valueChanged
  .CombineLatest(otherObservable, (value, other) => (value, other))
  .Subscribe(OnCombinationChanged);

```

### If it has a scheduler, provide one

There are a lot of operators that will have an overload for an `IScheduler`.  `IScheduler` is how you can tell Rx to marshall a given operation to a different thread context.  This is the heart of the asynchronous programming in Rx.  You have the ability to schedule operations on the thread you want.  This becomes way more powerful when you need to run something on a background thread.

```csharp
Observable.Start(() => 1, Scheduler.CurrentThread);
```

## How do I create an observable

### Observable.Start

`Observable.Start` will begin emitting an observable sequence with the value that you provide it. This is a great approach for returning values from functions that you want to initiate an observable sequence with the return value.  `Observable.Start` takes a thread scheduler.  Generally, when methods take thread schedulers you want to provide it a thread scheduler.

```csharp
{
  ...
  return Observable.Start(context, CurrentThreadScheduler.Instance);
}
```

### Observable.Return

`Observable.Return` is analogous to `Task.FromResult`.  Use it when you just want to return a single value, but you don't have an actual observable pipeline to chain it to.

```csharp
virtual IObservable<Unit> ExecuteCommand() => Observable.Return(Unit.Default);
```

### Observable.FromAsync

The below code emits a cold observable (a new observable sequence every time the method is called), with the return from the `_apiContract.Get()`.  Every time you call the method you will invoke the asynchronous call to the server to get data and return it to the pipeline.  Observables are inherently asynchronous.  While better suited for emitting multiple values over time, it is easy enough to wrap a single asynchronous notification like a `Task` in a single return.  This will allow you a consistent API for dealing with asynchronous requests and events.  You can now pipe asynchronous web service calls in with events and listen for one or the other.  Have a network request you want to fire in response to an event?  `Observable.FromAsync` can provide you a mechanism.

`Observable.FromAsync` will return an observable from the completion notification of a given `Task` or `Task<T>`

```csharp
  public IObservable<Unit> CallWebService() => Observable.FromAsync(() => _apiContract.Get());
```

### Observable.FromEvent

One of the things we do often in Rx .NET is pushing events downstream.  If you want to take a .NET event, and create an `EventHandler` that will surface an `IObservable` of that event type `Observable.FromEvent` is where you want to be.

```csharp
public IObserverable<NotifyCollectionChangedEventArgs> CollectionChanged =
 Observable.FromEvent<EvenHandler<NotifyCollectionChangedEventArgs>, NotifyCollectionChangedEventArgs>(
  eventHandler =>
  {
    void Handler(object sender, NotifyCollectionChangedEventArgs eventArgs) => eventHandler(eventArgs)
    return Handler;
  },
  x => thing.NotifyCollectionChanged += x,
  x => thing.NotifyCollectionChanged -= x);
```