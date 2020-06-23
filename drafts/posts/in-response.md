Title: INPC is an event that you respond to, even though you don't use it that way.
Drafted: 07/22/2017
Published: 01/26/2020
Tags:
    - Draft
---


https://reactivex.slack.com/archives/C02AJB872/p1592421056145100


reactiveui


Łukasz Szczygiełek Yesterday at 2:10 PM
Hi everybody! I'm new to ReactiveUi, I'm learning. I have an issue how to solve one thing. I made a SO post https://stackoverflow.com/questions/62417146/how-to-wait-for-observableaspropertyhelper-update-in-reactiveui - I don't know how to solve it and answer is not helpful (or I don't understand). I'm curious - maybe I described an issue wrong. If anybody would like to help me with this issue, than I will write more info. I go through reactiveui documentation and I didn't found similar case to mine.
Stack OverflowStack Overflow
How to wait for ObservableAsPropertyHelper update in ReactiveUI
I have a method in which I change property which triggers a change on other property which backing field is ObservableAsPropertyHelper type. Additionally filed which is ObservableAsPropertyHelper




9 replies

rlittlesii  5 hours ago
What is not helpful about the answer?
What I was trying to explain in 300 words or less is that your approach won't work for reactive.
What you seem to be wanting is to invoke a command in response to a property changing.  And I attempted to explain how that works.
If you're purpose is to invoke a command from the select, then you don't need to set the property helper.
The point of reactive programming is to delcare how your logic works and then allow the state changes to invoke your declarations.
The way you are looking at the problem is backwards.  Hence my response.
I understand the learning curve, what doesn't make sense about my response?

rlittlesii  5 hours ago
in .NET Rx Where the LINQ operator, can be applied to an Observable to "filter"
http://reactivex.io/documentation/operators/filter.html
So in your specific scenario if you need the method invocation to happen in a specific condition you would do
this.WhenAnyValue(x => x.CarModelName)
      .Where(x => /* some expression to filer the condition to what you want */
      .InvokeCommand(x =>  x => x.CarViewModel.Thing);

Łukasz Szczygiełek  2 hours ago
Thank you @rlittlesii for your time and response. I defenetly agree with you that my approach is wrong for reactiveui and I'm learning new way of thinking. I will probably introduce field to store a new configuration and I will attach to change on CarViewModel to load new configuration if field is set - it doesn't sound as great approach, but it's doable and it will work. I doubt that I can write it better by LINQ with filtering this part of code - maybe I will do but after some time with programming using reactiveui.

rlittlesii  2 hours ago
I want to understand better what your facing.  A lot of the trouble I had with learning Reactive Programming (which is the model that powers ReactiveUI), is changing how I approach a problem.
I had to shift to a more "event" based model.
"How does changing this thing affect all the other moving parts?"
If you can articulate your whole use case, I am happy to help you work through the code to get there.
I appreciate the learning curve, as I am still learning.  So if I can help, I am happy to.

rlittlesii  2 hours ago
I will probably introduce field to store a new configuration and I will attach to change on
You can do that from inside the same WhenAnyValue or you can change the notifications together outside that statement.
So now you are reacting to the state of both the properties. (edited) 

Łukasz Szczygiełek  23 minutes ago
I have a MainView.xaml which has:
- ComboBox to choose CarModelName,
- ViewModelViewHost to show view for CarViewModel.
Code looks like this:
    private string _carModelName;
    private readonly ObservableAsPropertyHelper<ICarModel> _carViewModel;
    public string CarModelName
    {
        get => _carModelName;
        set => this.RaiseAndSetIfChanged(ref _carModelName, value);
    }
    public ICarModel CarViewModel => _carViewModel.Value;
    this.WhenAnyValue(x => x.CarModelName)
        .Select<string, ICarModel>(x =>
        {
            switch (x)
            {
                case "VW":
                    return new VWModel();
                case "BMW":
                    return new BMWViewModel();
                default:
                    return null;
            }
        })
        .ToProperty(this, x => x.CarViewModel, out _carViewModel);
Using this code I have a scenario:
1. User can select CarModelName by ComboBox.
2. After selection he see a view which is connected with CarViewModel.
Changing CarModelName triggers change to CarViewModel - programmed in WhenAnyValue...ToProperty.
Now I want have a current scenario and add new one.
I want to have a button and it will perform such operations:
1. Read object:
    CarConfiguration
    {
        string Name;
        object Configuration;
    }
2. Then set CarModelName to CarConfiguration.Name (it will reevaluate CarViewModel and triggers View change in ViewModelViewHost).
3. Call method on CarViewModel.Load(CarConfiguration.Configuration);
I don't know how to remodel this to be more reactive. Thats why I think I will create a field _configuration and I will listen on CarViewModel change and then I will check if _configuration is null or not (CarViewModel changed indirectly by ComboBox should not call Load method), because in regular method I can't do something like this:
CarConfiguration conf = read();
CarModelName = conf.Name;
// calling it just after "CarModelName = conf.Name" is not working because CarViewModel is not yet updated and it's null
CarViewModel.Load(conf.Configuration);
I don't think that _configuration should be a reactive object, but if yes then I don't know how to do something like:
    private readonly ObservableAsPropertyHelper<CarConfiguration> _configuration;
    public CarConfiguration Configuration => _configuration.Value;
    // pseudo code - I don't know how to type such thing
    this.WhenAnyValue(x => x.Configuration)
        .Do(c => CarModelName = Configuration.Name)
        .WhenAnyValue(x => x.CarViewModel)
        .Do(x => CarViewModel.Load(Configuration.Configuration);
Sorry in advance if I write stupid things.

rlittlesii  8 minutes ago
Please, do not apologize for anything.  I see better what you are trying to do.  I am going to play with it in my sandbox and see if I can re-work it to how I would do it.
At a glance, some psuedo code.
this.WhenAnyValue(x => x.CarModelName, x => x.Configuration, (name, config) => new {} /* something that encapsulates the aggregation of these events to your need*/)
.Where(x => x.Configuration != null /*whatever other conditions you require*/)
.InvokeCommand(CarViewModel.Command);

rlittlesii  5 minutes ago
You're other method can set the name of CarModelName and then the WhenAnyValue will just "happen" because of the INPC.
I'd even go as far as potentially merging observables, because you have the observable generating _carViewModel you can compose it in a way that allows you to have two subscriptions to it, if that makes sense.  So you can set everything prior to the .ToProperty to a variable and reuse it.

rlittlesii  5 minutes ago
If I am speaking too detailed, and this doesn't make sense, again let me know.  Trying to help you see the problem different.