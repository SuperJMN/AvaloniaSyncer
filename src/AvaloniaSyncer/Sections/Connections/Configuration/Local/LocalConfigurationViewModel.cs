using System;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Local;

public class LocalConfigurationViewModel : ConfigurationViewModelBase
{
    public LocalConfigurationViewModel(Guid id, string name, IConnectionsRepository connectionsRepository) : base(id, name, connectionsRepository)
    {
    }
}