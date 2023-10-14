namespace AvaloniaSyncer.Sections.Connections;

public class LocalConfigurationViewModel : IConfiguration
{
    public string Name { get; }

    public LocalConfigurationViewModel(string name)
    {
        Name = name;
    }
}