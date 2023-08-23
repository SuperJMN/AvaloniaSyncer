using System;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.Local;

class LocalPlugin : IPlugin
{
    private readonly Maybe<ILogger> logger;

    public LocalPlugin(Maybe<ILogger> logger)
    {
        this.logger = logger;
        Settings = Maybe<IPluginSettings>.None;
    }

    public string Name => "Local";
    public Uri Icon => new Uri("avares://AvaloniaSyncer/Assets/sftp.png");

    public ISession Create()
    {
        return new Session(logger);
    }

    public Maybe<IPluginSettings> Settings { get; }
}