namespace AvaloniaSyncer.Plugins;

public interface IFileSystemPluginFactory
{
    public string Name { get; }
    public IFileSystemPlugin Create();
}