using System;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace AvaloniaSyncer.Plugins.Local;

class Plugin : IPlugin
{
    private readonly Maybe<ILogger> logger;
    private readonly Func<IFileSystem, IFolderPicker> folderPicker;

    public Plugin(Func<IFileSystem, IFolderPicker> folderPicker, Maybe<ILogger> logger)
    {
        this.logger = logger;
        this.folderPicker = folderPicker;
        Settings = Maybe<IPluginSettings>.None;
    }

    public string Name => "Local";
    public Uri Icon => new("avares://AvaloniaSyncer/Assets/sftp.png");

    public ISession Create()
    {
        return new SessionViewModel(folderPicker, logger);
    }

    public Maybe<IPluginSettings> Settings { get; }
}