namespace AvaloniaSyncer.Sections.Connections.Configuration.Android;

public class AndroidPlugin : IPlugin
{
    public string Name => "Android local";

    public IConfiguration CreateConfig(string name)
    {
        return new AndroidConfigurationViewModel(name);
    }
}