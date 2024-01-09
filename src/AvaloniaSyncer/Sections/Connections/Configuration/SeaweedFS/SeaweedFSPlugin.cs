using System;

namespace AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;

public class SeaweedFSPlugin : IPlugin
{
    public string Name => "SeaweedFS";

    public IConfiguration CreateConfig(IConnectionsRepository connectionsRepository, Action<ConfigurationViewModelBase> onRemove)
    {
        return new SeaweedConfigurationViewModel(Guid.NewGuid(), "SeaweedFS", new Uri("http://myhost:8888"), connectionsRepository, onRemove)
        {
            IsNew = true,
        };
    }
}