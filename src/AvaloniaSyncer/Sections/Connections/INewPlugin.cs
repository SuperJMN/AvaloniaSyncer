namespace AvaloniaSyncer.Sections.Connections;

public interface INewPlugin
{
    string Name { get; }
    IConfiguration CreateConfig(string name);
}