Title: ReactiveUI Snippets
Drafted: 06/14/2020
Published: 06/23/2020
Tags:
    - ReactiveUI
    - MVVM
    - Xamarin Forms
---

## That ReactiveUI stuff is neat, but it's so verbose!

I bet you didn't know ReactiveUI provides code snippets to make working with common parts of the framework easier.  If you are unfamiliar with snippets, you can read more about it on [Visual Studio](https://docs.microsoft.com/en-us/visualstudio/ide/code-snippets?view=vs-2019) or [JetBrains Rider](https://www.jetbrains.com/help/rider/Reference__Templates_Explorer__Live_Templates.html) documentation. In support of [Louis Matos’s Xamarin Month](https://luismts.com/code-snippetss-xamarin-month/#), focused on Code Snippets, here is some information on what ReactiveUI provides.

### How to get going?

ReactiveUI has a good amount of documentation.  Our website has everything from API documentation for the various libraries we support, to documentation on how to affect various features of the framework.  We even have a [getting started guide](https://reactiveui.net/docs/handbook/snippets/) for installing snippets into your IDE.

### Fun for IDE's of all ages

We are an equal opportunity project.  We support several of the major IDE's available in the .NET development space.

- [Resharper](https://github.com/reactiveui/ReactiveUI/tree/main/snippets/Resharper)
- [Rider](https://github.com/reactiveui/ReactiveUI/tree/main/snippets/Rider)
- [Visual Studio for Mac](https://github.com/reactiveui/ReactiveUI/tree/main/snippets/Visual%20Studio%20for%20Mac)
- [Visual Studio](https://github.com/reactiveui/ReactiveUI/tree/main/snippets/Visual%20Studio)

## Bindings

ReactiveUI provides coded bindings.  These exist primarily for frameworks that don't use XAML as a basis for UI generation but can be used on XAML pages as well in the code file containing the partial class (.xaml.cs).  ReactiveUI provides a strongly typed `BindingContext` called `ViewModel`.  This allows us to have fidelity into properties for binding.  You can use the non-typed, be prepared to cast the `BindingContext` in that case.

### ruiowb
#### One Way Bind
```csharp
this.OneWayBind(ViewModel, viewModel => viewModel.IsLoading, view => view.Loading.IsVisible);
```

### ruib
#### Bind

```csharp
this.Bind(ViewModel, viewModel => viewModel.SearchText, view => view.SearchBar.Text);
```

### ruibc
#### Bind Command

```csharp
this.BindCommand(ViewModel, viewModel => viewModel.ClickedCommand, view => view.Button, nameof(Button.Clicked));
```

## Properties
ReactiveUI has [Fody Property Change](https://www.nuget.org/packages/ReactiveUI.Fody).  If you aren't a fan of IL weaving, these next snippets are for you!  They provide properties that surface `INotifyPropertyChanged` events for `ReactiveObjects`, the base of `INPC` for ReactiveUI.

### ruiprop
#### Properties

```csharp
private Guid _id;
. . .
public Guid Id
{
    get => _id;
    set => this.RaiseAndSetIfChanged(ref _id, value);
}
```

### ruioaph
#### Observable As Property Helper
```csharp
private ObservableAsPropertyHelper<bool> _result;
public bool Result => _result.Value;
```

### ruicommand
#### Reactive Command
```csharp
public ReactiveCommand<Unit, Unit> NavigateCommand { get; }
```

## Interactions
[Interactions](https://reactiveui.net/docs/handbook/interactions) are their own thing.  They exist to allow you to invert control flow.  When you are in ViewModel logic and need to get information from the user, Interactions provides a truly decoupled (not with some odd abstraction) approach for you to capture it.  They are unit-testable by default, and I have not missed the inclusion of a dialog service being injected everywhere. If you want to see them in action I recently did a [Live Stream](https://www.youtube.com/watch?v=fBWwag-Jqvo) on the topic.

### ruiinteraction
#### Interactions 
```csharp
public Interaction<Exception, bool> ShowException = new Interaction<Exception, bool>();
```

There are a few more snippets that I don't find as much value in, but they exist.  I recently started adopting DynamicData.  The author did not provide snippets but did provide [code to show what you might want from a snippet](https://github.com/RolandPheasant/DynamicData.Snippets/tree/master/DynamicData.Snippets).  Who knows, maybe in my abundance of spare time, I might see if making some DynamicData snippets is in order.
