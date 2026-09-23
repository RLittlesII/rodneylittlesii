---
title: "Search with DynamicData and ReactiveUI"
published: 2020-06-22
drafted: 2020-06-22
draft: true
tags:
    - C#
    - DynamicData
    - ReactiveUI
---

```csharp
Func<ViewModelItem, bool> search(string searchTerm) =>
    viewModel =>
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return true;
        }

        return viewModel.AccountSearchString.Contains(searchTerm);
    };

var searchFilter =
    this.WhenAnyValue(x => x.SearchText)
        .Throttle(TimeSpan.FromMilliseconds(800), RxApp.TaskpoolScheduler)
        .DistinctUntilChanged()
        .Select(search);

_sourceCache.Connect()
            .RefCount()
            .Transform(x => new ViewModelItem(x))
            .AutoRefresh(x => x.IsActive)
            .Filter(searchFilter)
            .Sort(SortExpressionComparer<ViewModelItem>.Descending(x => x.IsActive).ThenByAscending(x => x.Id))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _accounts)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(ViewModelSubscriptions);
```