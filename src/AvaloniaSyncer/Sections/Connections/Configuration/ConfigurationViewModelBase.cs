using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.StringEditor;
using Zafiro.UI.Fields;

namespace AvaloniaSyncer.Sections.Connections.Configuration;

public abstract class ConfigurationViewModelBase : ReactiveValidationObject, IConfiguration
{
    private readonly IConnectionsRepository connectionsRepository;

    public ConfigurationViewModelBase(Guid id, string name, IConnectionsRepository connectionsRepository, Action<ConfigurationViewModelBase> onRemove)
    {
        this.connectionsRepository = connectionsRepository;
        Id = id;
        Name = new StringField(name);
        Name.Validate(s => !string.IsNullOrWhiteSpace(s), "Can't be empty");
        Remove = ReactiveCommand.Create(() => onRemove(this));
    }

    public ReactiveCommand<Unit, Unit> Remove { get; }

    public CombinedReactiveCommand<Unit, Unit> CommitAllFields => ReactiveCommand.CreateCombined(Fields.Select(f => f.Commit));
    public CombinedReactiveCommand<Unit, Unit> CancelAllFields => ReactiveCommand.CreateCombined(Fields.Select(f => f.Rollback));

    public IObservable<bool> CanSave => IsValid.CombineLatest(Observable.CombineLatest(this.WhenAnyValue(x => x.IsNew), IsDirty).Select(list => list.Any(b => b)), (isValid, hasFreshData) => isValid && hasFreshData);
    public IObservable<bool> IsDirty => Fields.Select(x => x.IsDirty).CombineLatest().Select(list => list.Any(isDirty => isDirty));
    protected abstract IEnumerable<IField> Fields { get; }

    [Reactive] public bool IsNew { get; set; }

    public ReactiveCommand<Unit, Unit> Save => ReactiveCommand.CreateFromTask(async () =>
    {
        await connectionsRepository.AddOrUpdate(Mapper.ToConnection(this));
        CommitAllFields.Execute().Subscribe();
        IsNew = false;
        return Unit.Default;
    }, CanSave);

    public CombinedReactiveCommand<Unit, Unit> Cancel => CancelAllFields;

    public Guid Id { get; }
    public StringField Name { get; }

    public IObservable<bool> IsValid => Fields.Select(x => x.IsValid).CombineLatest().Select(list => list.All(isValid => isValid));

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

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((ConfigurationViewModelBase) obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    protected bool Equals(ConfigurationViewModelBase other) => Id.Equals(other.Id);
}