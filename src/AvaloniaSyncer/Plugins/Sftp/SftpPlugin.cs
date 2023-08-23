using System;
using AvaloniaSyncer.Plugins.Local;
using AvaloniaSyncer.Plugins.Sftp.Configuration;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.Sftp;

class SftpPlugin : IPlugin
{
    private readonly Maybe<ILogger> logger;

    public SftpPlugin(Maybe<ILogger> logger)
    {
        this.logger = logger;
        Settings = new SettingsViewModel(logger);
    }

    public string Name => "SFTP";

    public Uri Icon => new Uri("avares://AvaloniaSyncer/Assets/sftp.png");
    
    public ISession Create()
    {
        return new SftpPluginViewModel(logger);
    }

    public Maybe<IPluginSettings> Settings { get; }
}