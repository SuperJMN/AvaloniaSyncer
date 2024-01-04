﻿using System;
using System.Reactive;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using ReactiveUI;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.StringEditor;
using Zafiro.UI.Fields;

namespace AvaloniaSyncer.Sections.Connections.Configuration;

public abstract class ConfigurationViewModelBase : ReactiveValidationObject, IConfiguration
{
    private readonly IConnectionsRepository connectionsRepository;

    public ConfigurationViewModelBase(Guid id, string name, IConnectionsRepository connectionsRepository)
    {
        this.connectionsRepository = connectionsRepository;
        Id = id;
        Name = new StringField { Initial = name };
        Name.AddRule(s => !string.IsNullOrWhiteSpace(s), "Can't be empty");
    }

    public ReactiveCommand<Unit, Unit> Save => ReactiveCommand.CreateFromTask(() => connectionsRepository.AddOrUpdate(Mapper.ToConnection(this)), IsValid);
    public Guid Id { get; }
    public StringField Name { get; }

    public abstract IObservable<bool> IsValid { get; }
    public abstract IObservable<bool> IsDirty { get; }

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

        return Equals((ConfigurationViewModelBase)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();
}