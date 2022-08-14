Title: Asynchronous Programming void return types with Reactive Commands
Drafted: 01/26/2020
Published: 02/27/2020
Tags:
    - Asynchronous Programming
    - Reactive Extensions
    - ReactiveUI
    - Task Parallel Library
---

# Let's play a game

I'm a big kid (I'm actually not big, so I guess a playful adult is more accurate), and I like to play games, care to indulge me in a quick game?  Let's play a game that I seem to lose at quite often.  "What does the code do?"

### Spot the problem with the following code!

```csharp
protected BaseViewModel(INavigationService navigationService)
        { 
            NavigateCommand = ReactiveCommand.Create<string>(NavigateCommandExecute);
            RegisterCanNavigateCommand(NavigateCommand );
            ...
        }
protected virtual void RegisterCanNavigateCommand<T, U>(ReactiveCommand<T, U> command) =>
           command.IsExecuting.Subscribe(
               x => this.CanNavigate= !x,
               _ => this.CanNavigate = true,
               () => this.CanNavigate= true
           );
protected virtual async void NavigateCommandExecute(string uri)
        {
            ...
            await this.NavigationService.NavigateAsync(uri);
            ...
        }
```

Before I point out flaws, I want to point out something I like about the code.  This code allows the visualization that the `Subscribe` provides an extension point for you to implement all three of the `IObserver` functions.

Okay, so you figure it out?  It took me a bit longer than I care to admit.

The main issues I found are as follows:

- `ReactiveCommand` has built execution safety, so the extension method is duplication
- `ReactiveCommand.Create()` API is used with an `async` execution.
- `async void` return type

## Creating Reactive Commands

When creating a `ReactiveCommand` selecting the correct `Create` overload matters.

- `ReactiveCommand.Create()` creates a command from a provided `Func` or `Action`.
- `ReactiveCommand.CreateFromTask()` creates a command from an asynchronous process that uses the Task Parallel Library (TPL) to provide a completion notification.
- `ReactiveCommand.CreateFromObservable()` creates a command from an Observable pipeline.

[ReactiveCommand Documentation](https://reactiveui.net/docs/handbook/commands)


## async Task vs async void

The documentation for 
[void](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/void) states the following:

> When used as the return type for a method, void specifies that the method doesn't return a value.

`ReactiveCommand.Create` creates an Action with a `void` return type.  In the above sample, the action will fire the Task, and not wait for completion because of the void return type.  This means that the TPL will not create a synchronization context to return to, and the action will either work or not.  No Aggregation of exceptions will happen.  Your action will "Fire and Forget".  This is because the use of `void` means there is no return.  This is a potential unintended side effect.

### Why you don't async void

Given two method signatures, what would the expected result be?

```csharp
public async Task FireAndReturn();
public async void FireAndReturn();
```

The first method will *potentially* go through the process of creating the TPL state machine, and create a place for the Synchronization Context to return.  This will allow for the method to fire and return to the calling context.  This means it can execute asynchronously on a different thread, and return to where it was executed.

The second method signature, while allowing for asynchronous processing on a different thread, will not return to the calling context.  The method lacks a return value because of the use of `void` as the return type.  This means that even though it won't block any other processing, you cannot expect to receive any information about its completion.  Why though? Why won't it return to the caller? Because `void` means the method does not actually return.

`Task` creates a place for a return, `void` by definition means there is no return value.  So when you use `async void` you are telling C# that you have no interest in completion information.  You simply want to execute the code.  While you can still get the benefit of multi-threading, you do not get the luxury the TPL provides with completion notifications.

### ReactiveCommand IsExecuting is not working?

The question was posed, why does the `IsExecuting` change before the work is complete?  Can you guess why?

In the above sample code, we created a `ReactiveCommand` with a `void` return type.  So based on what we've discussed what should be the expected behavior of our `IsExecuting` observable?  If the method is not actually returning a completion notification or returning anything at all, I would expect `IsExecuting` to return to false as soon as the method is invoked, not when the method completes.  We are using `void` return which means we have zero knowledge of the completion of the method completion.


## Asynchronous Programming != Task Parallel Library


> .NET pre async/await used to have a call back system.  You had a method that did work and a reference point for it to return to when it completed.  This was confusing.  So the TPL was invented.  It is syntactic sugar around the exact same thing.
Understanding asynchronous programming is important because Rx is a different way to model asynchronous programming.  So you have to pay attention to whether or not your method has a call back (completion notification).
In the TPL that is a Task.  So by doing async void you are effectively saying “Fire this thing asynchronously, but don’t return”.  This is called Fire and Forget.  If you want a notification.  The Task is the place where the asynchronous method will return to when it completes.

The above statement was from a Slack thread where I was attempting to address our code example for this post.  This simply means that Asynchronous Programming in .NET is more than the TPL.  The `async` and `await` keywords are just one way to perform Asynchronous Programming in C#.  It has been the default way for most programmers since the 4.0 version when it was introduced, but it is not the *only* way to handle this.  My personal favorite way to model asynchrony is using Observables.


## Thanks for Playing

This post shows us that understanding return types is important. C# language constructs can cause potential issues with our code if we are not diligent while using the tools provided properly.  I hope this simple example is enough to make you think twice about using `async void` as a return type.  If not, hopefully the Microsoft Documentation on [async await best practices can persuade](https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming) you.  Maybe next time we will talk more about how to model asynchronous programming outside the TPL, and take a look at how you can use Observables to handle asynchronous tasks in your code.