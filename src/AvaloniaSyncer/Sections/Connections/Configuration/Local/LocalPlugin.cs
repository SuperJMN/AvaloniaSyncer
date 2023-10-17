namespace AvaloniaSyncer.Sections.Connections.Configuration.Local;

public class LocalPlugin : INewPlugin
{
    public string Name => "Local";

    public IConfiguration CreateConfig(string name)
    {
        return new LocalConfigurationViewModel(name);
    }
}