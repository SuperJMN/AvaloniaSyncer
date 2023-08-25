using System;
using AvaloniaSyncer.Plugins.SeaweedFS_new.Settings;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.SeaweedFS_new;

class PluginViewModel : ViewModelBase, IPlugin
{
    private readonly Maybe<ILogger> logger;

    public PluginViewModel(Maybe<ILogger> logger)
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