using System;
using AvaloniaSyncer.Plugins.SeaweedFS.Settings;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.SeaweedFS;

public class Plugin : IPlugin
{
    private readonly Maybe<ILogger> logger;

    public Plugin(Maybe<ILogger> logger)
    {
        this.logger = logger;
        Settings = new SeaweedSettingsViewModel(logger);
    }

    public string Name => "SeaweedFS";

    public Uri Icon => new("avares://AvaloniaSyncer/Assets/seaweedfs.png");

    public ISession Create()
    {
        return new SessionViewModel(logger);
    }

    public Maybe<IPluginSettings> Settings { get; }
}