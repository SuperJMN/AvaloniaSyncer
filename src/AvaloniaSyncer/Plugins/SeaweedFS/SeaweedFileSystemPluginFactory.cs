using System;
using AvaloniaSyncer.Plugins.Local;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.SeaweedFS;

class SeaweedFileSystemPluginFactory : IFileSystemPluginFactory
{
    private readonly Maybe<ILogger> logger;

    public SeaweedFileSystemPluginFactory(Maybe<ILogger> logger)
    {
        this.logger = logger;
        Configuration = new Configuration.ConfigViewModel(logger);
    }

    public string Name => "SeaweedFS";

    public Uri Icon => new Uri("avares://AvaloniaSyncer/Assets/sftp.png");
    
    public IFileSystemPlugin Create()
    {
        return new SeaweedFSPluginViewModel(logger);
    }

    public Maybe<IPluginConfiguration> Configuration { get; }
}