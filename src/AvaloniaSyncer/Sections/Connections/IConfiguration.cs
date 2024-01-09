using System;
using System.Reactive;
using ReactiveUI;
using Zafiro.Avalonia.Controls.StringEditor;
using Zafiro.UI.Fields;

namespace AvaloniaSyncer.Sections.Connections;

public interface IConfiguration
{
    public Guid Id { get; }
    IObservable<bool> IsValid { get; }
    public Field<string> Name { get; }
    ReactiveCommand<Unit, Unit> Save { get; }
    ReactiveCommand<Unit, Unit> Remove { get; }
}