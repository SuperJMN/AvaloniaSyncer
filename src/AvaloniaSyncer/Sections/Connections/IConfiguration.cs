using System;
using System.Reactive;
using AvaloniaSyncer.Controls;
using ReactiveUI;

namespace AvaloniaSyncer.Sections.Connections;

public interface IConfiguration
{
    public Guid Id { get; }
    IObservable<bool> IsValid { get; }
    public StringProperty Name { get; }
    ReactiveCommand<Unit, Unit> Save { get; set; }
}