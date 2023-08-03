namespace AvaloniaSyncer;

class SftpPluginFactory : IFileSystemPluginFactory
{
    public string Name => "SFTP";
    public IFileSystemPlugin Create()
    {
        return new SftpPlugin();
    }
}