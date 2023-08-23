using System;
using AvaloniaSyncer.Plugins.Local;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.SeaweedFS;

class SeaweedPlugin : IPlugin
{
    private readonly Maybe<ILogger> logger;

    public SeaweedPlugin(Maybe<ILogger> logger)
    {
        this.logger = logger;
        Settings = new Configuration.SettingsViewModel(logger);
    }

    public string Name => "SeaweedFS";

    public Uri Icon => new Uri("avares://AvaloniaSyncer/Assets/sftp.png");
    
    public ISession Create()
    {
        return new SeaweedFSPluginViewModel(logger);
    }

    public Maybe<IPluginSettings> Settings { get; }
}