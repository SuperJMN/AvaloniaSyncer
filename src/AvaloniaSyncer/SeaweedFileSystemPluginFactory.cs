using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer;

class SeaweedFileSystemPluginFactory : IFileSystemPluginFactory
{
    private readonly Maybe<ILogger> logger;

    public SeaweedFileSystemPluginFactory(Maybe<ILogger> logger)
    {
        this.logger = logger;
    }

    public string Name => "SeaweedFS";
    public IFileSystemPlugin Create()
    {
        return new SeaweedFSPlugin(logger);
    }
}