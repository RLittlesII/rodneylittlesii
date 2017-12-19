Title: Activity Control with ReactiveUI
Drafted: 12/18/2017
Published: 05/10/18
Tags:
 - Reactive UI
 - Xamarin Forms
---

# The Designer Challenge

The designer decided to challenge me and force me outside my comfort zone.  We decided to put a spinner in a button to indicate work being done.  First thought, "I don't think Xamarin.Forms does that by default."  The more I stared at the design the more I thought about all the state changes that would occur.

My "button" would have the following states:
- Initial state with default text
- Activity state which would indicate to the user the button was busy
- A completed state which would indicate to the user that I had done some work
- Potentially an error state to let the user know that some problem had occured


# This sounds like a job for ReactiveUI!

I am not sure if you are familiar with [ReactiveUI](https://reactiveui.net/) or [Reactive Extensions](http://reactivex.io/).  Reactive Extensions are a set of extension methods for observables that allow developers to handle multiple streams of event driven data with LINQ operations.  This is a very fancy way of saying LINQ to Events.  You've heard of LINQ to Sql, and LINQ to objects, well this is LINQ over observable streams of data.

ReactiveUI is the perfect way for me to handle [Mutable](http://www.dictionary.com/browse/mutable) State.  I can wire up observers to the state changes on the UI and react to those changes, and enforce business rules.  This is not a foreign concept.  Proponents of [Functional Reactive Programming](https://en.wikipedia.org/wiki/Functional_reactive_programming) talk about this concept all the time.  Many JavaScript frameworks use this paradigm these days.  The programming language doesn't need to be a Functional Language in order to use the concepts.

# Observer Pattern

Back to basics.  We all understand the [Observer Pattern](https://en.wikipedia.org/wiki/Observer_pattern) right?  Or do we just throw an `ObservableCollection<T>` in the code and pray our `INotifyPropertyChanged` events fire?  I know I did at first.  I *knew* the pattern, but didn't connect the dots to the power of the pattern in practice.  ReactiveUI has helped me better understand how reacting to a series of events can provide clarity to the business needs and my code.

> **Disclaimer**: I am by no means an expert in the art of mutable state.  I am learning to tame state by reacting to its changes.


# Get off the soap box and give me code!!!
I created an Activity Button Control that responds to the UI.  I explicitly wired this one up to a login page example.  But the control is a reusable element that you can use how you see fit.  There are defintely flaws, and I welcome any feedback on what I could have done better.

### [ReactiveUI Activity Button Control](https://github.com/RLittlesII/ActivityButton)