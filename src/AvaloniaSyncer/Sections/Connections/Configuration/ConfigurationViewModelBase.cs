using System;
using System.Reactive;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.StringEditor;
using Zafiro.UI.Fields;

namespace AvaloniaSyncer.Sections.Connections.Configuration;

public abstract class ConfigurationViewModelBase : ReactiveValidationObject, IConfiguration
{
    public ConfigurationViewModelBase(Guid id, string name, IConnectionsRepository connectionsRepository)
    {
        Id = id;
        Name = new StringField(name);
        Name.AddRule(s => !string.IsNullOrWhiteSpace(s), "Can't be empty");
        Save = ReactiveCommand.CreateFromTask(() => connectionsRepository.AddOrUpdate(Mapper.ToConnection(this)), IsValid);
    }

    public ReactiveCommand<Unit, Unit> Save { get; set; }
    public Guid Id { get; }
    public IObservable<bool> IsValid => this.IsValid();
    public StringField Name { get; }

    protected bool Equals(ConfigurationViewModelBase other) => Id.Equals(other.Id);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((ConfigurationViewModelBase) obj);
    }

    public override int GetHashCode() => Id.GetHashCode();
}