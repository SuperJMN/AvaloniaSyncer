using System;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Sftp;

public class SftpPlugin : IPlugin
{
    public string Name => "SFTP";

    public IConfiguration CreateConfig(IConnectionsRepository connectionsRepository, Action<ConfigurationViewModelBase> onRemove)
    {
        return new SftpConfigurationViewModel(Guid.NewGuid(), "SFTP", new SftpConnectionParameters("", 22, "", ""), connectionsRepository, onRemove)
        {
            IsNew = true,
        };
    }
}