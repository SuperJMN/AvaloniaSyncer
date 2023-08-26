using System;
using AvaloniaSyncer.Plugins.Sftp.Settings;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.Sftp;

public class Plugin : IPlugin
{
    private readonly Maybe<ILogger> logger;

    public Plugin(Maybe<ILogger> logger)
    {
        this.logger = logger;
        Settings = new SettingsViewModel(logger);
    }

    public string Name => "SFTP (SSH)";

    public Uri Icon => new("avares://AvaloniaSyncer/Assets/seaweedfs.png");

    public ISession Create()
    {
        return new SessionViewModel(logger);
    }

    public Maybe<IPluginSettings> Settings { get; }
}