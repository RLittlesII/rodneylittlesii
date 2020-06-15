Title: Timing, Ordering and Scheduling.
Drafted: 06/14/2020
Published: 06/14/2020
Tags:
    - Reactive Extensions
    - .NET
    - Asynchronous
---

# I think I am running late

#### System.Reactive.Concurrency

`Scheduler`

###### Scheduler.Immediate
```csharp
/// <summary>
/// Gets a scheduler that schedules work immediately on the current thread.
/// </summary>
public static ImmediateScheduler Immediate => ImmediateScheduler.Instance;
```

##### Scheduler.CurrentThread
```csharp
/// <summary>
/// Gets a scheduler that schedules work as soon as possible on the current thread.
/// </summary>
public static CurrentThreadScheduler CurrentThread => CurrentThreadScheduler.Instance;
```

##### Scheduler.Default
```csharp
/// <summary>
/// Gets a scheduler that schedules work on the platform's default scheduler.
/// </summary>
public static DefaultScheduler Default => DefaultScheduler.Instance;
```

`ImmediateScheduler.Instance`

`Scheduler.CurrentThread`