namespace AvaloniaSyncer.Sections.Connections.Configuration.Sftp;

public class SftpPlugin : IPlugin
{
    public string Name => "SFTP";

    public IConfiguration CreateConfig(string name)
    {
        return new SftpConfigurationViewModel(name);
    }
}