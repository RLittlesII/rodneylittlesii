Title: Modeling and managing Mobile Application State
Drafted: 11/22/2022
Published: 11/22/2022
Tags:
    - Functional Programming
    - Mediator Design Pattern
    - MediatR
    - Reactive Extensions
    - Stateless
    - State Design Pattern
---

# The Problem

Mobile application state is a sticky wicket.  You receive events that can mutate the application state with no user interaction like reduced network signal. Users can receive a phone call, or other events which could put your app in a background state.  Events from outside the application (the operating system, the hardware) might change the state of the running application. What happens if these events conflict?  How do we handle them cleanly when the events might depend on each other?
These events transition your application to some state for some period of time.  We have an entire system that needs to "Wake Up", "Power Down", or "Run Offline" and we need to respond to the event and transition the mobile application to the proper state so it doesn't crash!  I have solved this problem a few ways over several mobile applications.  Still looking for a better way to solve the problem.

**Disclaimer:** The application I am solving this for has very *interesting* requirements because of all the background services that are happening and how they process.  Application scope and life cycles have a huge impact on the application, processing certain things in certain states can cause the application to crash.  If your application doesn't have these concerns, this *may* seem like over engneering, and that is okay!

**Note:** This post is about more than my choice of library.

# Application State

  The more I looked at this problem the more I realized I was making a [State Transition Diagram](https://en.wikipedia.org/wiki/State_diagram).

|      State      |      Event      |       State     |
| --------------- | --------------- | --------------- |
| Offline         | Connecting      | Online          |
| Online          | Disconnecting   | Offline         |
| Background      | Start           | Foreground      |
|                 |                 |                 |

 I realized I can use a [State Machine](https://en.wikipedia.org/wiki/Finite-state_machine) to model application state.  What better library to use for this than [Stateless](https://github.com/dotnet-state-machine/stateless)?  I have used it to build various aspects of various systems.  It is a fantastic library, feature-rich, and works for everything I have ever thrown at it.  Everything from a Volunteer Application Wizard to Workflow Engines, Stateless has been a dream to work with!  There are a few key things I need to solve for, and guidelines I want to attempt to follow as I model this state.

## The thing™

- Should adhere to the State Design Pattern (SDP)
- State transitions can be the result of Rx, not the source.
- Should encapsulate state transition handler execution before releasing the application event to the rest of the application for response

## Modeling using Stateless

Stateless allows for a typed state and a typed trigger.  Most examples I see use an `enum` for this.  So defining a set of states and transitions (triggers), is as simple as defining the below.

#### States

```csharp
public enum ApplicationMachineState
{
    /// <summary>
    /// The initial state of the application before any other state can be determined.
    /// </summary>
    Initial,

    /// <summary>
    /// Application in Background
    /// </summary>
    Background,

    /// <summary>
    /// Application in Foreground
    /// </summary>
    Foreground
}
```

#### Trigger

```csharp
public enum ApplicationMachineTrigger
{
    /// <summary>
    /// Start the application.
    /// </summary>
    Start,

    /// <summary>
    /// Stop the application.
    /// </summary>
    Stop,

    /// <summary>
    /// Deeplink application launch.
    /// </summary>
    Deeplink,

    /// <summary>
    /// Background notification launch.
    /// </summary>
    Notification
}
```

# Transitioning Application State

The application state will change based on various factors.  Whether the user gets a phone call, or if the device losses connection, these concerns are represented as events.  So we need to observe the events and call the state machine method that accepts the correct event type.
Stateless allows us to define triggers that accept parameters.  We are going to use this feature to encapsulate the State Machine trigger mechanism and force the internal transition When the application triggers an event, it will call a function on the State Machine in an attempt to modify the internal state and drive a behavior change.

### Handle Transitions

There is a particular method we are using from Stateless that is the lynch pin of this whole approach.

```csharp
public void OnTransitionCompleted(Action<StateMachine<TState, TTrigger>.Transition> onTransitionAction)
```

This methods documentation states

> Registers a callback that will be invoked every time the statemachine transitions from one state into another and all the OnEntryFrom etc methods have been invoked

This means we can use the state machines `OnEntryFrom` pass in an `ApplicationStateEvent` allow the mediator to execute the correct handlers and after it completes, we signal the state change to observers.  This checks a requirements box and is a reason why we are putting any patterns around this problem.

It allows us to model external state with an event, express application state as a declarative machine, and handle application state transitions in a cohesive manner.  If you need to add an operation that has to process when the application is in the background, you just add a handler of the appropriate type!

```csharp
/// <summary>
/// Connect the application.
/// </summary>
/// <param name="event">The connect event.</param>
public void Connect(GainedSignalEvent @event) =>  // Recieve the event.
    Fire(_connect, @event);

    ...

Configure(NetworkMachineState.Online)
    .Permit(NetworkMachineTrigger.Disconnected, NetworkMachineState.Offline)
    // The declared machine will call the `PublishStateEvent` which will execute the correct handlers.
    .OnEntryFrom(_connect, connectivityChangedEvent => PublishStateEvent(connectivityChangedEvent)) 
    .OnEntry(CommonEntry)
    .OnExit(CommonExit);
```

## Modeling Application Events

dotnet provides a lot of our system events as event handlers.  Being a fan of Reactive Programming, I chose to model the events as a stream over time as opposed to individual state announcements.  I will be creating two interfaces, one to expose Network State events and another that provides Application State events.

### Network Signal

Create a default interface on top of an `IObservable<NetworkStateChangedEvent>` so we can answer questions about the implementation in one place.

```csharp
public interface INetworkState : IObservable<NetworkStateChangedEvent>
{
    IObservable<NetworkStateChangedEvent> WhereHasSignal() =>
        this
            .DistinctUntilChanged(changedEvent => changedEvent.NetworkAccess)
            .Where(changedEvent => changedEvent.HasSignal())
            .DistinctUntilChanged()
            .Publish()
            .RefCount();

    IObservable<NetworkStateChangedEvent> WhereHasNoSignal() =>
        this
            .DistinctUntilChanged(changedEvent => changedEvent.NetworkAccess)
            .Where(changedEvent => !changedEvent.HasSignal())
            .DistinctUntilChanged()
            .Publish()
            .RefCount();
}
```

### Application Lifecycle

I am chosing to model Xamarin.Forms application state.  If you want a more platform specific experience you would likely model your observables differently.  Again, using default interfaces on top of an observable

```csharp
public interface IApplicationLifecycleState : IObservable<LifecycleState>
{
    IObservable<LifecycleState> Initializing() =>
        this.Where(state => state is LifecycleState.Initializing);

    IObservable<LifecycleState> Pausing() =>
        this.Where(state => state is LifecycleState.Pausing);

    IObservable<LifecycleState> Starting() =>
        this.Where(state => state is LifecycleState.Starting);

    IObservable<LifecycleState> Resuming() =>
        this.Where(state => state is LifecycleState.Resuming);
}

public enum LifecycleState
{
    Initializing,
    Starting,
    Pausing,
    Resuming
}
```

# Managing Application State

For this task I'll create a single instance monitor that acts as a router.  It will process global state events as they occur, and convert them into something the application can consume for our purposes.  It encapsulates the machines and the event producer, when events are observed the coresponding state machine trigger fires and state application state has the chance to transition.

## Monitor Application State

```csharp
public class ApplicationStateMonitor : DisposableBase
{
    public ApplicationStateMonitor(IApplicationStateEvents applicationStateEvents,
        ApplicationStateMachine applicationStateMachine,
        NetworkStateMachine networkStateMachine)
    {
        applicationStateEvents
            .OfType<GainedSignalEvent>()
            .Subscribe(gainedSignal => networkStateMachine.Connect(gainedSignal))
            .DisposeWith(Garbage);

        applicationStateEvents
            .OfType<LostSignalEvent>()
            .Subscribe(lostSignal => networkStateMachine.Disconnect(lostSignal))
            .DisposeWith(Garbage);

        applicationStateEvents
            .OfType<InitializeApplicationEvent>()
            .Subscribe(initializeApplication => applicationStateMachine.Initialize(initializeApplication))
            .DisposeWith(Garbage);

        applicationStateEvents
            .OfType<StartApplicationEvent>()
            .Subscribe(startApplication => applicationStateMachine.Start(startApplication))
            .DisposeWith(Garbage);

        applicationStateEvents
            .OfType<StopApplicationEvent>()
            .Subscribe(stopApplication => applicationStateMachine.Stop(stopApplication))
            .DisposeWith(Garbage);

        State = applicationStateMachine
            .StateChanged
            .CombineLatest(networkStateMachine.StateChanged,
                (appState, networkState) => StateFactory(appState, networkState))
            .StartWith(ApplicationState.Default);

        static ApplicationState StateFactory(ApplicationMachineState appState, NetworkMachineState networkState) =>
            new(appState == ApplicationMachineState.Foreground, networkState == NetworkMachineState.Online);
    }

    public IObservable<ApplicationState> State { get; private set; }
}

public record ApplicationState(bool Foreground, bool Connected)
{
    public static ApplicationState Default { get; } = new(false, false);
}
```

## Modeling Transition Handlers

I want to observe an incoming event, based on the event type execute a set of developer defined functions, and complete the transition to the new state.  I thought about implementing a messaging queue for this, then I realized I was making it harder.  I can just use a [Mediator Pattern](https://refactoring.guru/design-patterns/mediator/csharp/example) to broadcast a signal, and whatever handlers I have available can just execute.  [MediatR](https://github.com/jbogard/MediatR) here I come!  I will have to extend it slightly because I want to use Observables.  I define the following interface to notify handlers when events are produced.

```csharp
public interface IApplicationStateMediator
{
    IObservable<Unit> Notify<TEvent>(TEvent notification)
        where TEvent : IStateEvent;
}
```

Handlers are generically typed to events which allow MediatR to resolve a set of handlers to execute.

```csharp
public interface IApplicationStateHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IStateEvent
{
    IObservable<Unit> Handle(TEvent stateEvent);
}
```

## Changing State
When the state does transition, the registered event handlers will get notified and handle the provided event.  This allows us to have 

```csharp
void PublishStateEvent(ApplicationStateEvent? stateEvent)
{
    if (stateEvent != null)
    {
        using var _ =
            applicationStateMediator
                .Notify(stateEvent)
                .Subscribe();
    }
}
```

# Mobile State is hard

This is the first iteration of this approach. I intend to grow this solution and see how it responds.  I am going to throw a few real world scenarios at the sample and see if all the boxes get checked.  Managing state is hard.  Harder still when you don't model the problem and understand the goals before you start writing code. Don't be discouraged if you don't get it right the first time, this is my third attempt at solving this problem, hopefully my last.  Keep modeling the thing™ until it fits and does more good than bad in the system.
