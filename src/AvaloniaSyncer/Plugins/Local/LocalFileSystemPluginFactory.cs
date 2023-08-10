using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.Local;

class LocalFileSystemPluginFactory : IFileSystemPluginFactory
{
    private readonly Maybe<ILogger> logger;

    public LocalFileSystemPluginFactory(Maybe<ILogger> logger)
    {
        this.logger = logger;
    }

    public string Name => "Local";
    public IFileSystemPlugin Create()
    {
        return new LocalPlugin(logger);
    }
}