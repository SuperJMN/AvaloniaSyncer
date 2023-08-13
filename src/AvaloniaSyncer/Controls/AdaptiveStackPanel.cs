using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;

namespace AvaloniaSyncer.Controls;

public class AdaptiveStackPanel : StackPanel
{
    public AdaptiveStackPanel()
    {
        this.WhenAnyValue(view => view.Bounds)
            .Sample(TimeSpan.FromSeconds(0.2), AvaloniaScheduler.Instance)
            .Select(GetOrientation)
            .Do(orientation =>
            {
                Orientation = orientation;
            })
            .Subscribe();
    }

    private Orientation GetOrientation(Rect rect)
    {
        if (rect.Width > 400)
        {
            return Orientation.Horizontal;
        }

        return Orientation.Vertical;
    }
}