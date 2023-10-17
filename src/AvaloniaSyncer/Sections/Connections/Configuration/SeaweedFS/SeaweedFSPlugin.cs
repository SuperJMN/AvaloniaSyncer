namespace AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;

public class SeaweedFSPlugin : INewPlugin
{
    public string Name => "SeaweedFS";

    public IConfiguration CreateConfig(string name)
    {
        return new SeaweedConfigurationViewModel(name);
    }
}