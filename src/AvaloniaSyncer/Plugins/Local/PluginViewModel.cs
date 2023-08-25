using System;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.Local;

class PluginViewModel : ViewModelBase, IPlugin
{
    private readonly Maybe<ILogger> logger;

    public PluginViewModel(Maybe<ILogger> logger)
    {
        this.logger = logger;
        Settings = Maybe<IPluginSettings>.None;
    }

    public string Name => "Local";
    public Uri Icon => new Uri("avares://AvaloniaSyncer/Assets/sftp.png");

    public ISession Create()
    {
        return new SessionViewModel(logger);
    }

    public Maybe<IPluginSettings> Settings { get; }
}