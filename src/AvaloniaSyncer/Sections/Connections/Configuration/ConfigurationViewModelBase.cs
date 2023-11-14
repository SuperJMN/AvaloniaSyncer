using System;
using System.Reactive;
using System.Reactive.Linq;
using AvaloniaSyncer.Controls;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Mixins;

namespace AvaloniaSyncer.Sections.Connections.Configuration;

public class ConfigurationViewModelBase : ReactiveValidationObject, IConfiguration
{
    public ConfigurationViewModelBase(Guid id, string name, IConnectionsRepository connectionsRepository)
    {
        Id = id;
        Name = new StringProperty(name);
        Name.ValidationRule(x => x.Temp, s => !string.IsNullOrWhiteSpace(s), "Can't be empty");
        Save = ReactiveCommand.CreateFromTask(() => connectionsRepository.AddOrUpdate(Mapper.ToConnection(this)));
        this.WhenAnyValue(x => x.Name.Value).Skip(1).ToSignal().InvokeCommand(Save);
    }

    public ReactiveCommand<Unit, Unit> Save { get; }
    public Guid Id { get; }
    public IObservable<bool> IsValid => this.IsValid();
    public StringProperty Name { get; }
}