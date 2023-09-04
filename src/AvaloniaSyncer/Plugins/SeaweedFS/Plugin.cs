using System;
using AvaloniaSyncer.Plugins.SeaweedFS.Settings;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace AvaloniaSyncer.Plugins.SeaweedFS;

public class Plugin : IPlugin
{
    private readonly Maybe<ILogger> logger;
    private readonly Func<IFileSystem, IFolderPicker> folderPicker;

    public Plugin(Func<IFileSystem, IFolderPicker> folderPicker, Maybe<ILogger> logger)
    {
        this.logger = logger;
        this.folderPicker = folderPicker;
        Settings = new SettingsViewModel(logger);
    }

    public string Name => "SeaweedFS";

    public Uri Icon => new("avares://AvaloniaSyncer/Assets/seaweedfs.png");

    public ISession Create()
    {
        return new SessionViewModel(folderPicker, logger);
    }

    public Maybe<IPluginSettings> Settings { get; }
}