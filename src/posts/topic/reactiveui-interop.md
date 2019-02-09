Title: Interoperability with ReactiveUI
Drafted: 02/08/2019
Published: 02/13/2019
Tags:
 - Mvvm
 - ReactiveUI
 - Xamarin.Forms
---

# One MVVM Framework above all others

I was at the local Xamarin user group and the topic of Mvvm came up.  One of the members was answering a question and stated that Prism was the only option for MVVM frameworks for Xamarin.  After choking down my disgust, I chimed in refuting the statement and listed several MVVM frameworks that work with the Xamarin platform.  Among them FreshMvvm, MvvmCross, MvvmLight and topped it off with ReactiveUI.

#### Achievement Unlocked: Open can of worms!

During that same meetup, I presented [Sextant](https://github.com/giusepe/Sextant) which is a Xamarin.Forms navigation library built on Reactive Extensions (Rx).  A library created from a blog post by [Kent Boogaart](https://kent-boogaart.com/blog/custom-routing-in-reactiveui) on Xamarin.Forms navigation services.  While answering questions on Sextant, someone asked me **"Why do I think Rx isn't more popular?"**.  Watching the pitched softball, I grinned and prepared to knock it out of the park.

#### Disclaimer: If you are reading this, the following statements probably don't apply to you

I believe that Rx is not popular in the C# community because C# isn't a functional language.  Even though many recent platform features are making C# more functional, C# developers don't do functional.  The reality is that Rx is hard, my experience has been that most C# developers don't do hard.  The learning curve is steep, the chance for mistakes high, but the payoff is well worth the investment.

If you are a JavaScript developer in today's world.  Things are more inherently reactive.  JavaScript promises and other aspects of the language make the subscription to observable streams more palpable.  That said, I think C# developers should learn how to model asynchrony as observables.  It is a common practice in a lot of other languages and is a skill that will transcend language specific constructs such as the [Task Parallel Library](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl).

# What's in a framework?

I went on record recently as stating that all MVVM frameworks are created equal.  What I mean by this, you chose an MVVM framework based on the features that meet your business requirements and align with your technologies.  For this reason, I don't believe any framework is really the "best".  We make the right decision until we unlock some hidden platform feature that forces us to start making compromises and submitting to the will of the framework.  Usually, at this point in a project, I start to second guess my life choices, while I struggle to make the framework do things the way I want.

The extensibility of a framework is a selling point for me.  I prefer frameworks that have an opinion but allow me to push my opinions harder than theirs.  I do believe that making a decision on a framework will potentially box your entire application into doing things the way that framework dictates.  So if you can provide me with functionality in a microtised manner, allowing me to consume only the bits I want, then I will test the opinions and build an application my way.

I use ReactiveUI because I think that Functional Reactive Programming is a great paradigm for managing mutable state.  If you have built a modern UI application recently, you will appreciate that responding to and managing changes in application state become increasingly more complex the more features you tack onto a screen.  ReactiveUI doesn't really box me into doing things the "ReactiveUI way", but using the full power of the framework is predicated on the technology of Reactive Extensions.  I can pick and chose how I build my application and add as much or as little Rx as I require.

### General List of Mvvm Platform features

- **View Model Navigation**: The ability to navigate from ViewModel to ViewModel.  Most UI frameworks consider Navigation a UI concept and expect that you will handle navigation in the view layer.  Frameworks that allow you to use ViewModel navigation circumvent that paradigm and let you handle navigation in the View Model via some injected Navigation Service.
- **Dependency Inversion**: Dependency Inversion is the general concept that a framework handles dependency registration with an Ioc container.
- **View Bindings**: Most UI frameworks have view bindings that can be done at the code behind.  If you are used to XAML binding and using a binding engine, this concept allows you to move bindings out of the XAML and bind the View elements to View Model properties in the View code behind.
- **Application State Management**: Application State is the concept of making sure that session information is persisted between application executions.  Have you ever used an application that doesn't allow you to pick up where you left off when you move it to the background?

# Extending ReactiveUI to another framework
I went home that night trying to answer the question **"How do I make ReactiveUI more consumable without having to convert an application to ReactiveUI?"**  My first Xamarin.Forms project was built on FreshMvvm because it is a lightweight framework that isn't extremely opinionated. It was selected because it provided View Model navigation and constructor injection features.

The first thing I did when I got the chance was to rub some Rx on it.  FreshMvvm uses a concrete object for it's ViewModel and subsequent navigation I couldn't easily just create a `ReactiveObject` and make things work.  Luckily for me, the creator of ReactiveUI believes in depending on abstractions and not concretions.  I realized as long as I could extend from `IReactiveObject` I could harness the power of ReactiveUI and not take all the opinions of ReactiveUI.

#### Eureka moment

Opened Visual Studio.  File new solution (or dotnet new if you are into that sort of thing), and a few hours of Google Fu and Code Juijitsu later, [ReactiveUI.Interop](https://www.github.com/RocketSurgeonsGuild/ReactiveUI.Interop) was born.  This library was based on the original FreshMvvm implementation I created expanded to MvvmCross and Prism.  Now you can use any of these frameworks as your framework of choice while still leveraging ReactiveUI and the `WhenAny` operator suite.  This should make reactive programming more consumable to people who have already made the architectural decision to use a framework with a different opinion.  My hope with this library is that people will use it to discover the power of Rx, and learn how ReactiveUI is a powerful tool for reacting to state.

So if you've heard about ReactiveUI and Reactive Extensions, but are too committed to the framework you know and love, give [ReactiveUI.Interop](https://www.nuget.org/packages?q=ReactiveUI.Interop) a try.  If your preferred framework isn't implemented, create an issue on the repository and lets work together to get you reacting to state change and winning the war against mutable state!