using System;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Local;

public class LocalPlugin : IPlugin
{
    public string Name => "Local";

    public IConfiguration CreateConfig(IConnectionsRepository connectionsRepository)
    {
        return new LocalConfigurationViewModel(Guid.NewGuid(), "Local", connectionsRepository);
    }
}