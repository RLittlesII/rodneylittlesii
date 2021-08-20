Title: Disposing of ReactiveCommands to adhere to the IDisposable contract
Drafted: 07/22/2017
Published: 10/22/2020
Tags:
    - MVVM
    - ReactiveUI
    - Reactive Extensions
    - Xamarin Forms
---

# IDisposable

Anything that implements `IDisposable` should have it's `.Dispose()` method called by the entity that constructed it.

# Object Disposed Exception

- long running task

[Issue 1854 Comment](https://github.com/reactiveui/ReactiveUI/issues/1854#issuecomment-453166660)


# ReactiveCommand

```csharp
    [Fact]
    public async Task ReactiveCommandThrowObjectDisposedException()
    {
        var disposable = new CompositeDisposable();
        var semaphore = new SemaphoreSlim(0);

        var task = semaphore.WaitAsync();

        var command = ReactiveCommand.CreateFromTask(() => task).DisposeWith(disposable);

        var result = Record.Exception(() => Observable.Return(Unit.Default)
            .InvokeCommand(command)
            .DisposeWith(disposable));

        disposable.Dispose();

        await Task.Delay(200)
            .ConfigureAwait(false);

        // Assert
        result.Should()
            .BeOfType<ObjectDisposedException>();
    }
```


# Subject.OnNext

- Throws a ODE

System.Reactive
```csharp
    /// <summary>
    /// Notifies all subscribed observers about the arrival of the specified element in the sequence.
    /// </summary>
    /// <param name="value">The value to send to all currently subscribed observers.</param>
    public override void OnNext(T value)
    {
        var observers = Volatile.Read(ref _observers);
        if (observers == Disposed)
        {
            _exception = null;
            ThrowDisposed();
            return;
        }
        foreach (var observer in observers)
        {
            observer.Observer?.OnNext(value);
        }
    }
```

# The caller can't be the disposer


    - The thing™ that calls the object being disposed shouldn't be responsible for it's disposal.