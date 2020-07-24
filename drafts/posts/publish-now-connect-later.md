Title: Defer subscription till when you need it using IConnectableObservable
Drafted: 04/17/2020
Published: 06/22/2020
Tags:
    - C#
    - .NET
    - Reactive Extensions
---

# What is this IConnectableObservable?

A few months ago I had a revelation blog post where someone smart helped me understand something I didn't.  That whole post was dedicated to understanding publishing, and connecting to observable sequences. [Publishing Reference Counts for Connected Observables](../posts/topic/rx-publish-refcount).  I would recommend understanding what `IConnectableObservable` is before going too far down the page.

# The problem with subscriptions
So now that we've set the above disclaimer, what is the problem with subscriptions?  They are technically method invocations.  So when a `Subscribe` happens on an Observable, it is live, reading notifications from it's stream.  Each `Subscribe()` is effectively another method invocation, meaning he allocation of another event handler. `Subscribe()` will immediately force the Observable pipeline to build and listen for notifications. Each `Subscribe()` could potentially be connected to a different observable (depending on how the pipeline was created).Is this always the desired behavior?  What if my user needs the ability to pause listening to the stream?  Maybe it's a stream of music and they want to pause.  Maybe it's a stream of events, that only matter in the morning.  Maybe it's a download you don't want to automatically trigger.  Whatever it is, sometimes we need to delay the emission of notificatons, without changing where we execute our `Subscribe()` statement.


## Solving that concern with Connect()

`Connect()` allows you to produce an `IConnectableObservable` from an `IObservable`.

Benefits
- I can add all my subcriptions and they won't be hot until I call `Connect()`
- All my downstream subscribers are getting the same observable

```csharp
public class MyViewModel
{
    IConnectableObservable _connectable;
    IDisposable _disposable = Disposabe.Empty;

    public MyViewModel()
    {
        _connectable = observabe.Publish();
        _connectable
            .Where(x => x...)
            .SelectMany(x => x...);

        _connectable
            .Subscribe();
    }

    public void Listen()
    {
        _disposable = _connectable.Connect();
    }

    public void Disconnect()
    {
        _disposable?.Dispose();
    }
}
```
