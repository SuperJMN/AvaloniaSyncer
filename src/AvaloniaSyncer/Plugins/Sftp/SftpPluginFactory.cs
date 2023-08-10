using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.Sftp;

class SftpPluginFactory : IFileSystemPluginFactory
{
    private readonly Maybe<ILogger> logger;

    public SftpPluginFactory(Maybe<ILogger> logger)
    {
        this.logger = logger;
    }

    public string Name => "SFTP";
    public IFileSystemPlugin Create()
    {
        return new SftpPlugin(logger);
    }
}