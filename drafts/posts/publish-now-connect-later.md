Title: Defer subscription till when you need it using IConnectableObservable
Drafted: 04/17/2020
Published: 04/17/2020
Tags:
    - C#
    - .NET
    - Reactive Extensions
---

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