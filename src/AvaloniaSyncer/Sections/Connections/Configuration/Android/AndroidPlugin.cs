using System;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Android;

public class AndroidPlugin : IPlugin
{
    public string Name => "Android local";

    public IConfiguration CreateConfig(IConnectionsRepository connectionsRepository)
    {
        return new AndroidConfigurationViewModel(Guid.NewGuid(), connectionsRepository);
    }
}