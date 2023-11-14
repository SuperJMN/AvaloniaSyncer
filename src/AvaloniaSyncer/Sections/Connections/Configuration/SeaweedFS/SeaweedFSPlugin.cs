using System;

namespace AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;

public class SeaweedFSPlugin : IPlugin
{
    public string Name => "SeaweedFS";

    public IConfiguration CreateConfig(IConnectionsRepository connectionsRepository)
    {
        return new SeaweedConfigurationViewModel(Guid.NewGuid(), "SeaweedFS", connectionsRepository);
    }
}