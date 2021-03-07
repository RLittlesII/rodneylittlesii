Title: I Think therefore, I Declare
Drafted: 10/22/2020
Published: 10/22/2020
Tags:
    - C#
    - .NET
    - Reactive Extensions
---

## Je pense donc, je suis

When I was younger I looked at a lot of the philiosophy/mathemetician types that wrote books, and I studied french.  This quote always resonated with me.[Rene Descartes](https://en.wikipedia.org/wiki/Cogito,_ergo_sum) coined the term "I think, therefore I am.".  Indulge me while I play on this to make my way the long way around a point?

## Functional programming is hard

Making the move from imperative to declarative programming is difficult.  One of the most difficult aspects is that you truly have to invert the way you look at the code.  If you go back to [State Transition]() diagrams and look at functional programming as a set of states and transitions, you might draw a connection that you are just responding to entering and exiting state.  This is similar to the [State Design Pattern](),  where classes behaviors change based on the state of the internal object.

### It's delcarative boogie woogie oogie
This is the imperative vs. declarative conversation.  It's uncomfortable.  The best way I can get to brass tax, imperative is the MPC.  It controls and knows all the things to run the system.  Declarative is many autonomous bits that have as close to a single responsibility as possible (so many levels to SRP).  This means that when the optimum state is achieved I know to do my job.  This can also build [cohesion](https://en.wikipedia.org/wiki/Cohesion_(computer_science)) in your system by grouping like atomic functions to satisfy the concern.

Declarative programming provides an approach for this.  By declaring what a small part of the system should do when the external state of the system changes, you can react to the external change, and allow it to change your internal state, there by changing your behavior to the system.  So now instead of "What does the code need to do?", we can shift our mental model.  "I am an automonous bit, when something happens, I am supposed to do ...".  This allows up to understand the boundary, the external state change, and the new behavior we will project after processing the change.

### In response

Most modern programmers have handled an HTTP request in some fashion.  So you should be familiar with the concept of a Request/Response.  Think of an event being raised as a request.  You're handler invocation is your response.  The current limitation of the C# event system is that you have to respond to the event immediately.  With Reactive Extensions you can change that.  You can use LINQ over a stream of events.  So back to our mental model shift, "When the event is raise, I will, throttle the events, respond to the change, and broadcast new state to external listeners".

I can now talk about my automonous bits responses to the transitions of state, because that's basically what a property changed event is, a transition from one state to another.  You're response to that state, and change in behavior is what makes it so similar to the afor-mentioned state design pattern.

### Consume and produce the minimal change

Part of the sell of TDD and BDD is that you write the minimum lines of code to get a test to pass.  This helps reduce code, and hopefully drive adherance to Single Responsibility Principle (SRP).  I think Reactive Extensions gives you another tool to help keep you on that straight and narrow path.  If we can respond to the exact condition required to update a single item,  I think we may have found a bit more depth to SRP.  Now we can say, "I only want to broadcast these state changes, on a rainy day."  LINQ operators you know and love `Select`, `Where`, `Take`, `Skip`, with new ones like `Throttle`, `DistinctUntilChanged`, `Merge`, and `CombineLatest` to name a few.

These operators can be chained together creating a pipeline of decisions that need to be satisfied to get to the state change pay off!  Imagine only mutating state when you have to, not as a side effect of other code?!  All this power and more can be yours at the cost of shifting your mental model to consume and produce less state changes instead of maintaining them.

### State is the enemy, don't manage state, let it mutate

My early poorly place mention of the Master Control Program.  Well, that is the example of managing state.  It is having a method that is responsible for changing massive amounts of state so all the logic can be in one place.  Well.  What is the most important method on any class?  The constructor!  Guess where you declare al your autonomous pieces of Rx wizardy?!  If you guessed the constructor, then you're still reading.

Managing state for the program is much harder than responding to the current state of the program with a set of pre-canned responses.  I read somewhere years ago that a state machine seems complex at first, but as complexity grow, and requirements change, and code scales you'll be glad you had it.  Reactive Extensions allow you to build customized state machines, built specifically for your requirements.  So as your requirements scale your code will scale easier with it.  Generally new requirements means new states, new transitions, and new behaviors.  So having a customisable state machine could be a pretty handy tool.

## Composition is King