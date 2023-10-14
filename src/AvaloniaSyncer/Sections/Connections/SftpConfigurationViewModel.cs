namespace AvaloniaSyncer.Sections.Connections;

public class SftpConfigurationViewModel : IConfiguration
{
    public string Name { get; }

    public SftpConfigurationViewModel(string name)
    {
        Name = name;
    }
}