Title: Thread marshalling in Reactive Programming - Observe On vs Subscribe On
Drafted: 03/03/2022
Published: 06/23/2022
Tags:
    - Asynchronous Programming
    - C#
    - Reactive Extensions
---

# What's in a thread?

In my opinion, thread marshalling is one of the hardest concepts in asynchronous programming.  Understanding multi-threading, and further, taking control of multi-threading in functional programming can be a daunting task.  It demands knowledge of where you came from, where you are going and when you are scheduled to arrive.  Some operators expose overloads for passing a thread context and it isn't always obvious you should use it.  All things that we don't generally consider, matter when we want to harness the power of things like the `TaskpoolScheduler`.  The relevant question to me was, what is asynchronous programming, and does asynchrony guarantee multiple threads?

# async/await does not mean multi-threaded

Shocked?!  I was.  The [documentation](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/task-asynchronous-programming-model#BKMK_Threads) suggests that you are not **guaranteed** a new thread when using `async`/`await`.  So even though the keywords lead you towards parallelism, and asynchrony seems synonymous with multi-threading, they are not.  Saving you the hassle of a full explanation, when you `await` a `Task` in C# it will continue to process normally until the program execution requires the return value, the application _may_ spawn a new thread and continue processing what it can until the return value is required for further execution.  So it gives the illusion of normal execution while reading the code, but in reality, there is a state machine that captures thread context for return processing.  This is where things like `ConfigureAwait(continueOnCapturedContext: true);` come into play.  This allows an awaited `Task` to return to the context it came from or any context.

An analogy I have heard and used for this:  `.ConfigureAwait(true)` is similar to waiting on the number 42 bus, and the bus id 1234.  `.ConfigureAwait(false)` is waiting for _any_ number 42 bus.  This is the extent of the multi-threading support when awaiting most tasks.  If you simply want to "ensure" something gets executed on a new thread you can use either `Task.Run(() => { })` or `Task.Factory.StartNew(() => { })`.

# Schedulers define When and Where

Do you remember Time Cop?  If not, there is an important principle stated in the theatrics which is relevant to this discussion. [Pauli Exclusion Principle](https://en.wikipedia.org/wiki/Pauli_exclusion_principle).  In essence, two identical matter particles cannot occupy the same space.  What does this have _anything_ to do with Asynchronous Programming?  Well, while we envision things on multiple threads and concurrent, two operations cannot occur at the same time on the same thread.  This now adds an element of time complexity to our conversation which was supposed to be as simple as `async`/`await`.  This is represented in Reactive Extensions by the `IScheduler` interface.

```csharp
public interface IScheduler
{
    DateTimeOffset Now { get; }
    IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action);
    IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action);
    IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action);
}
```

#### Schedulers have clocks

`IScheduler` has a clock that tells you _when_ operations should take place as well as _where_ they should take place.  A given scheduler implementation will ensure that operations are queued to happen on a thread, at the proper time to ensure we do not violate the concerns.  This isn't theoretical physics, but two operations cannot take place at the same time on the same thread.  They can take place at the same time on different threads, constituting multi-threaded.  Why does this matter?  How might this manifest?  It matters because more advanced multi-threading techniques might result in a deadlock on a thread, or a thread that never returns to the calling context.  In a desktop or mobile application, it might manifest as a crash.  In a web application perhaps waiting indefinitely for some JavaScript to complete.

# Marshalling?  Who called the police?!

Horrible American law enforcement joke aside.  The concept of "thread marshalling" basically means we move to a thread context. [Marshalling](https://en.wikipedia.org/wiki/Marshalling_%28computer_science%29) is the process of transforming the in-memory representation of an object into a form for transmission.  In the context of threads, it's preparing a delegate to execute on one thread versus another.  So the act of marshalling operations from one thread context to another allows us to ensure our delegate executes on the correct thread.  In functional programming `.Subscribe()` and its overloads, are the asynchronous call back function that executes when a promise is fulfilled.  "At some point in the future, execute this action".  The joy of functional programming is, _whenever_ the condition is met, the call back is executed (unless explicitly specified).  General asynchronous programming points to a single execution call back.  Sometimes we want to ensure that the callback is marshaled to the correct thread.  The question we need to answer is, where will the work be done?

# Observe On

This is where semantics make things tricky.  When we think of observing, we think of watching something happen.  So it would follow logically when an observable happens, we want to watch it on a thread.  It doesn't make us think of where the action will be executed.  Unfortunately, this is the operator you want to ensure execution on a given thread.  You can use `.ObserveOn(IScheduler scheduler)` multiple times on a given Reactive pipeline.  When you want to marshal your pipeline to a given thread, you use the `.ObserveOn(IScheduler scheduler)` operator directly before the operator you want marshaled to execute on a given thread.

```csharp
...
    .Timer(TimeSpan.FromSeconds(3), false)
    .TakeWhile(_ => _ >= TimeSpan.Zero)
    .ObserveOn(TaskPoolScheduler.Default)
    .ForEachAsync(_ => _logger.Information(_.ToString()));
...
```

Below is a pipeline that shows a bit more advanced concern for thread marshalling.  It shows that you can use multiple `ObserveOn(IScheduler scheduler)` operators in a single pipeline to transition various portions of execution on different thread contexts.

```csharp
...
    .ObserveOn(Scheduler.Immediate) // Everything down stream of this ObserveOn will happen on the immediate scheduler, until the thread is marshalled.
    .Select(isRunning => isRunning ? timer : Observable.Never<TimeSpan>())
    .Switch()
    .Publish()
    .RefCount()
    .ObserveOn(Scheduler.Default) // Everything down stream of this ObserveOn will happen on the default scheduler.
...
```

- Can be used all the times™
- Position matters

# Subscribe On

This is the hard part, because of the `.Subscribe()` operator we think "SubscribeOn, sure that's where I would schedule the work to be done".  This is a trick.  The [documentation](https://reactivex.io/documentation/operators/subscribeon.html) states
> The SubscribeOn operator designates which thread the observable will begin operating on, no matter at what point in the chain of operators that operator is called.

What does that mean?  It means the observable sequence will begin emitting a value on the specified thread.  The first value will be emitted on the specified thread.  It does not mean all work will execute on that thread.  It _could_ if you do not do any additional thread marshalling, but it is not guaranteed.  It also implies that you can put the `.SubscribeOn(IScheduler scheduler)` anywhere in the observable sequences operator chain and it will have the same outcome, it will begin emitting values on that scheduler.

- Can be used once
- Position does **not** matter

# When to use which?

Now that you've read the lengthy explanation, and indulged my ignorance, I've provided a simple cheat sheet to explain the differences between `SubscribeOn(IScheduler scheduler)` and `ObserveOn(IScheduler scheduler)`.

| SubscribeOn | ObserveOn  |
| ----------- | ---------- |
| Notification origination | Notification processing |
| One per pipeline | Multiple per pipeline |
| Position Agnostic | Position Dependent |

#### Further Reading

- [Configure Await](https://devblogs.microsoft.com/dotnet/configureawait-faq)
- [Concurrency and Parallelism in Asynchronous Programming](https://stackoverflow.com/questions/4844637/what-is-the-difference-between-concurrency-parallelism-and-asynchronous-methods)
