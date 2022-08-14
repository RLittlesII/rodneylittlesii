Title: INotifyPropertyChanged is not the same as INotifyCollectionChanged
Drafted: 04/17/2020
Published: 05/13/2020
Tags:
    - C#
    - DynamicData
    - .NET
    - dotnet
    - MVVM
    - Reactive Extensions
---

# I set the property for INPC and it isn't updating when I add to the list?!

When I first started using `ObservableCollection` I thought that it surfaced individual change notifications, and if bound to a property, would receive those notifications and update accordingly.  Man, was I wrong?  Recently I've had this conversation several times, and it usually starts with understanding what `INotifyPropertyChanged` does.  So what does it do?

## INotifyPropertyChanged

In the [MSFT Documentation](https://docs.microsoft.com/en-us/dotnet/framework/winforms/how-to-implement-the-inotifypropertychanged-interface)

[INotifyPropertyChanged](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged) is an interface that defines a `PropertyChangedEventHandler` that notifies a listener that the state of the property has mutated.  This is a simple and powerful tool.  Many UI kits understand and have support for `INotifyProperyChanged`, which makes this an ideal way to setup MVVM implementations.  Your ViewModel is bound to the View, which has a BindingEngine that listens for properties that are changed. Traditionally the implementation of `INotifyPropertyChanged` will invoke some `PropertyChangedEventHandler` when the setter is called on that property, like below.

```csharp
public event PropertyChangedEventHandler PropertyChanged;

public string MyProperty
{
    get => _myProperty;
    set
    {
        _myProperty = value;
        NotifyPropertyChanged();
    }
}

private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
{
    if (PropertyChanged != null)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

`NotifyPropertyChanged` will invoke the `PropertyChangedEventHandler` and lets listeners know that state has mutated.  Based on the code above, we see that this changed notification only happens when the property is set.  This means the state has to be set and as a result, we notify listeners of the new state.  So why the confusion?

## INotifyCollectionChanged
We seem to expect that state changes *to* the list property should propagate as state changes *of* the list property.  If that were true, we'd expect to see every character change notification for a `string` as it is just a `char[]` in disguise! So why do we expect that adding items to a collection property would trigger this change?  `ObservableCollection`.  For some reason, we as developers have been saddled with the notion that because the contents of this property changes that the `INotifyPropertyChanged` would handle this case.  I thought so until I looked at what it extends.  `INotifyCollectionChanged`.

`ObservableCollection` implements `INotifyCollectionChanged` which will notify us when the collection has a mutated state.  This means creates, adds, updates, deletes.  Very few of us are explained this difference when we come to MVVM.  We are told about `INotifyPropertyChanged`, and shown techniques to clear lists and to create a new list every time so the property changed argument will fire.  In the context of a UI tool kit, this would force say a `ListView` to potentially redraw every `ViewCell` on the screen.

The code below surfaces internal changes to the collection and invokes it's corresponding property changed handler.  This will send a signal to any property change subscribers that the state has mutated.  This coupled to your binding engine through your ViewModel should propagate the changes desired.  The code is verbose!  It solves most of our concerns but leaves a large footprint in our code.  Yes, it uses Reactive Extensions (spoiler, I like Rx), but the amount of work required to get that change notification is a lot.  Also, we are not handling individual changes, we are only notifying to the binding engine that *something* in the list changed.  Can we do it better?

```csharp
ObservableCollection<string> collection = new ObservableCollection<string>(Enumerable<string>.Empty);

var changedEvents =
    Observable
        .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eventHandler =>
        {
            void Handler = (object sender, NotifyCollectionChangedEventArgs args) => eventHandler(args);
            return Handler;
        },
        x => collection.CollectionChanged += x,
        x => collection.CollectionChanged -= x);

changedEvents
    .Subscribe(_ =>
    { 
        NotifyPropertyChanged("MyProperty");
    })
```

## Why not use DynamicData?

The below code will surface the changes of the internal list to the bound `_items` list.  This means anytime we manipulate the items *in* the list, we will raise a property changed notification for the property *of* the list.  DynamicData handles this when it binds to any `ObservableCollection` derivative.  This provides a smooth clear understanding of list contents so that a given binding engine will know to add, remove, or update any item in the list when the state mutates.  This is a much cleaner approach to me than most of the variants I have tried.

```csharp
private ReadOnlyObservableCollection<MyViewItemModel> _items = new ReadOnlyObservableCollection<string>();

 public MyViewModel()
 {
    _sourceCache
        .Connect()
        .RefCount()
        .Transform(x => new MyViewItemModel(x))
        .Bind(out _items)
        .DisposeMany()
        .Subscribe();
 }

public ReadOnlyObservableCollection<MyViewItemModel> Items => _items;
```

The main take away isn't to use [DyamicData](https://github.com/reactiveui/DynamicData), although you should.  The whole point of this rant is that when you update a list, the property that list represents doesn't know about what happens inside.  So you need to surface the change notifications so the property knows to update your binding.  DynamicData just provides a clean, fluent and observable approach to making that possible.

#### References

- [INotifyPropertyChanged](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged)
- [PropertyChanged](https://github.com/LeeCampbell/RxCookbook/blob/master/Model/PropertyChange.md)
- [INotifyCollectionChanged](https://docs.microsoft.com/en-us/dotnet/api/system.collections.specialized.inotifycollectionchanged)
- [CollectionChange](https://github.com/LeeCampbell/RxCookbook/blob/master/Model/CollectionChange.md)