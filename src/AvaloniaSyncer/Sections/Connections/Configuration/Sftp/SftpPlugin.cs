using System;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Sftp;

public class SftpPlugin : IPlugin
{
    public string Name => "SFTP";

    public IConfiguration CreateConfig(IConnectionsRepository connectionsRepository)
    {
        return new SftpConfigurationViewModel(Guid.NewGuid());
    }
}