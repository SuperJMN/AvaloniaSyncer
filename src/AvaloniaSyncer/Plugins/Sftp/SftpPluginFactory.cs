namespace AvaloniaSyncer.Plugins.Sftp;

class SftpPluginFactory : IFileSystemPluginFactory
{
    public string Name => "SFTP";
    public IFileSystemPlugin Create()
    {
        return new SftpPlugin();
    }
}