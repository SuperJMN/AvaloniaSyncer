using System;
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
        Settings = Maybe<IPluginSettings>.None;
    }

    public string Name => "SeaweedFS";
    public Uri Icon => new Uri("avares://AvaloniaSyncer/Assets/seaweedfs.png");

    public ISession Create()
    {
        return new SessionViewModel(logger);
    }

    public Maybe<IPluginSettings> Settings { get; }
}