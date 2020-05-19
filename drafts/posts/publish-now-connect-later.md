Title: Defer subscription till when you need it using IConnectableObservable
Drafted: 04/17/2020
Published: 04/17/2020
Tags:
    - C#
    - .NET
    - Reactive Extensions
---

# The problem with subscriptions
- Each `Subscribe()` is effectively another method invocation.  In terms of EventHandlers, it is the allocation of another event handler.
- `Subscribe()` will immediately force the Observable pipeline to build and listen for notifications.
- Each `Subscribe()` could potentially be connected to a different observable (depending on how the pipeline was created)

## Solving that concern with Connect()

`Connect()` allows you to produce an `IConnectableObservable` from an `IObservable`.

Benefits
- I can add all my subcriptions and they won't be hot until I call `Connect()`
- All my downstream subscribers are getting the same observable

```csharp
var connectable = observabe.Publish();
connectable.Where(x => ...).SelectMany(x => x...);

...
public void Listen() {
_disposable = connectable.Connect();
}

public void Disconnect() {
_disposable.Dispose();
}
```