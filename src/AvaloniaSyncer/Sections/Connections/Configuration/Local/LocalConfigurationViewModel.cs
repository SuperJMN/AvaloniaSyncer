using System;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Local;

public class LocalConfigurationViewModel : ConfigurationViewModelBase
{
    public LocalConfigurationViewModel(Guid id, string name, IConnectionsRepository connectionsRepository) : base(id, name, connectionsRepository)
    {
    }

    public override IObservable<bool> IsValid => Name.IsValid;
    public override IObservable<bool> IsDirty => Name.IsDirty;
}