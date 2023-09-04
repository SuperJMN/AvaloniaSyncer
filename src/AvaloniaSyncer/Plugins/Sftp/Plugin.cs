using System;
using AvaloniaSyncer.Plugins.Sftp.Settings;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace AvaloniaSyncer.Plugins.Sftp;

public class Plugin : IPlugin
{
    private readonly Func<IFileSystem, IFolderPicker> folderPickerFactory;
    private readonly Maybe<ILogger> logger;

    public Plugin(Func<IFileSystem, IFolderPicker> folderPickerFactory, Maybe<ILogger> logger)
    {
        this.folderPickerFactory = folderPickerFactory;
        this.logger = logger;
        Settings = new SettingsViewModel(logger);
    }

    public string Name => "SFTP (SSH)";

    public Uri Icon => new("avares://AvaloniaSyncer/Assets/seaweedfs.png");

    public ISession Create()
    {
        return new SessionViewModel(folderPickerFactory, logger);
    }

    public Maybe<IPluginSettings> Settings { get; }
}