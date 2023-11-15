using System;
using System.Collections.Specialized;
using System.Linq;
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
                .Where(x => x.EventArgs.NewItems.Count != AssociatedObject.ItemsView.Count && x.EventArgs.Action == NotifyCollectionChangedAction.Add)
                .Select(x => x.EventArgs.NewItems!.Cast<object>().Last()))
            .Switch();

        selectionUpdater = lastAddedItem
            .Do(item => AssociatedObject.SelectedItem = item)
            .Subscribe();

        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        selectionUpdater?.Dispose();
        base.OnDetaching();
    }
}