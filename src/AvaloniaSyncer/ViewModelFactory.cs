using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using AvaloniaSyncer.Plugins;
using AvaloniaSyncer.Plugins.Local;
using AvaloniaSyncer.Sections.Connections;
using AvaloniaSyncer.Sections.Explorer;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using AvaloniaSyncer.Sections.NewSync;
using AvaloniaSyncer.Sections.Settings;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Pickers;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.Notifications;

namespace AvaloniaSyncer;

public class ViewModelFactory
{
    public ViewModelFactory(IApplicationLifetime applicationLifetime, Visual control, Maybe<ILogger> logger)
    {
        Logger = logger;
        NotificationService = new NotificationService(new WindowNotificationManager(TopLevel.GetTopLevel(control)));
        DialogService = Zafiro.Avalonia.Dialogs.DialogService.Create(applicationLifetime, configureWindow: Maybe<Action<ConfigureWindowContext>>.From(ConfigureWindow));
        Plugins = AvailablePlugins();
        Clipboard = new ClipboardViewModel();
        TransferManager = new TransferManagerViewModel { AutoStartOnAdd = true };
    }

    public Maybe<ILogger> Logger { get; set; }

    public IClipboard Clipboard { get; set; }

    public NotificationService NotificationService { get; }

    private IPlugin[] Plugins { get; }

    private IDialogService DialogService { get; }

    public ITransferManager TransferManager { get; }

    public SettingsSectionViewModel GetSettingsViewModel()
    {
        return new SettingsSectionViewModel(Plugins);
    }

    public SyncronizationSectionViewModel GetSynchronizationSection()
    {
        var fsConn = new LocalFileSystemConnection("Local");
        return new SyncronizationSectionViewModel(
            new ReadOnlyObservableCollection<IFileSystemConnection>(
                new ObservableCollection<IFileSystemConnection>(new IFileSystemConnection[]
                {
                    fsConn, 
                    new SeaweedFileFileSystemConnection("SeaweedFS", new Uri("http://192.168.1.29:8888"), Logger)
                })), 
            DialogService, 
            NotificationService, 
            Clipboard, 
            TransferManager, Logger);
    }

    public ExplorerSectionViewModel GetExploreSection()
    {
        return new ExplorerSectionViewModel(NotificationService, Clipboard, TransferManager, Logger);
    }

    private static void ConfigureWindow(ConfigureWindowContext context)
    {
        context.ToConfigure.Width = context.Parent.Bounds.Width / 1.5;
        context.ToConfigure.Height = context.Parent.Bounds.Height / 1.5;
    }

    private IPlugin[] AvailablePlugins()
    {
        var logger = Maybe.From(Log.Logger);
        return new IPlugin[]
        {
            new Plugin(fs => new FolderPicker(DialogService, fs, NotificationService, Clipboard, TransferManager), logger),
            new Plugins.SeaweedFS.Plugin(fs => new FolderPicker(DialogService, fs, NotificationService, Clipboard, TransferManager), logger),
            new Plugins.Sftp.Plugin(fs => new FolderPicker(DialogService, fs, NotificationService, Clipboard, TransferManager), logger)
        };
    }

    public async Task<ConnectionsSectionViewModel> GetSettingsViewModel2()
    {
        return new ConnectionsSectionViewModel(await LoadFromFile(), NotificationService, DialogService);
    }

    private async Task<IConnectionsRepository> LoadFromFile()
    {
        var store = new ConfigurationStore(() => File.OpenRead("Connections.json"), () => File.OpenWrite("Connections.json"));
        var loadResult = await store.Load();
        var result = loadResult.Map(enumerable => new ConnectionsRepository(enumerable.Select(x => Mapper.ToSystem(x, Logger)), Logger));
        return result.GetValueOrDefault(new ConnectionsRepository(Enumerable.Empty<IFileSystemConnection>(), Logger));
    }
}