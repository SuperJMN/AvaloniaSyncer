using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.StringEditor;

namespace AvaloniaSyncer.Sections.Connections.Configuration;

public abstract class ConfigurationViewModelBase : ReactiveValidationObject, IConfiguration
{
    public ConfigurationViewModelBase(Guid id, string name, IConnectionsRepository connectionsRepository)
    {
        Id = id;
        Name = new EditableString(name);
        Name.ValidationRule(s => !string.IsNullOrWhiteSpace(s), "Can't be empty");
        Save = ReactiveCommand.CreateFromTask(() => connectionsRepository.AddOrUpdate(Mapper.ToConnection(this)), IsValid);
    }

    public ReactiveCommand<Unit, Unit> Save { get; set; }
    public Guid Id { get; }
    public IObservable<bool> IsValid => this.IsValid();
    public EditableString Name { get; }

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