using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using ReactiveUI;

namespace AvaloniaSyncer.Controls;

public class StringEditor : TemplatedControl
{
    public static readonly StyledProperty<StringProperty> StringPropertyProperty = AvaloniaProperty.Register<StringEditor, StringProperty>(
        nameof(StringProperty));

    public StringProperty StringProperty
    {
        get => GetValue(StringPropertyProperty);
        set => SetValue(StringPropertyProperty, value);
    }

    public static readonly StyledProperty<bool> IsEditVisibleProperty = AvaloniaProperty.Register<StringEditor, bool>(nameof(IsEditVisible), true);

    public bool IsEditVisible
    {
        get => GetValue(IsEditVisibleProperty);
        set => SetValue(IsEditVisibleProperty, value);
    }

    public StringEditor()
    {
        this.WhenAnyValue(s => s.IsEditVisible)
            .Where(b => !b)
            .Do(_ => StringProperty.Cancel.Execute(null))
            .Subscribe();
    }
}