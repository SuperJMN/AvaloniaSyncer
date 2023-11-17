using System;
using System.Reactive;
using ReactiveUI;
using Zafiro.Avalonia.Controls.StringEditor;

namespace AvaloniaSyncer.Sections.Connections;

public interface IConfiguration
{
    public Guid Id { get; }
    IObservable<bool> IsValid { get; }
    public EditableString Name { get; }
    ReactiveCommand<Unit, Unit> Save { get; set; }
}