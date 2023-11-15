using System;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Android;

public class AndroidConfigurationViewModel : ConfigurationViewModelBase
{
    public AndroidConfigurationViewModel(Guid id, string name, IConnectionsRepository connectionsRepository): base(id, name, connectionsRepository)
    {
    }
}