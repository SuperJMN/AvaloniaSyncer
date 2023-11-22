using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;

namespace AvaloniaSyncer;

public class SelectNewItemBehavior : Behavior<SelectingItemsControl>
{
    private IDisposable? selectionUpdater;

    protected override void OnAttached()
    {
        if (AssociatedObject is null)
        {
            return; 
        }
        
        var lastAddedItem = AssociatedObject
            .WhenAnyValue(x => x.ItemsView)
            .Select(view => view as INotifyCollectionChanged)
            .Select(changed => Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    handler => changed.CollectionChanged += handler,
                    handler => changed.CollectionChanged -= handler)
                .Where(HasAddedItems)
                .Select(x => AssociatedObject.ItemsView.Cast<object>().Last()))
            .Switch();

        selectionUpdater = lastAddedItem
            .Do(item => AssociatedObject.SelectedItem = item)
            .Subscribe();

        base.OnAttached();
    }

    private bool HasAddedItems(EventPattern<NotifyCollectionChangedEventArgs> x)
    {
        if (AssociatedObject is null)
        {
            return false; 
        }

        if (x.EventArgs.Action == NotifyCollectionChangedAction.Reset && AssociatedObject.ItemsView.Count == 1)
        {
            return true;
        }

        var allItemsAreNew = x.EventArgs.NewItems != null && x.EventArgs.NewItems.Count != AssociatedObject.ItemsView.Count;
        var isAdd = x.EventArgs.Action == NotifyCollectionChangedAction.Add;
        return allItemsAreNew && isAdd;
    }

    protected override void OnDetaching()
    {
        selectionUpdater?.Dispose();
        base.OnDetaching();
    }
}