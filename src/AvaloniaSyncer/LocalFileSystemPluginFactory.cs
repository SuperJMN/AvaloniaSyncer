namespace AvaloniaSyncer;

class LocalFileSystemPluginFactory : IFileSystemPluginFactory
{
    public string Name => "Local";
    public IFileSystemPlugin Create()
    {
        return new LocalPlugin();
    }
}