using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using AvaloniaSyncer.Plugins;
using AvaloniaSyncer.Plugins.Local;
using AvaloniaSyncer.Sections.Explorer;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using AvaloniaSyncer.Sections.NewSync;
using AvaloniaSyncer.Sections.Settings;
using AvaloniaSyncer.Sections.Synchronization.Sync;
using CSharpFunctionalExtensions;
using DynamicData;
using Microsoft.Build.Utilities;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Pickers;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.Notifications;

namespace AvaloniaSyncer;

public class ViewModelFactory
{
    private readonly Maybe<ILogger> logger;

    public ViewModelFactory(IApplicationLifetime applicationLifetime, Visual control, Maybe<ILogger> logger)
    {
        this.logger = logger;
        NotificationService = new NotificationService(new WindowNotificationManager(TopLevel.GetTopLevel(control)));
        DialogService = Zafiro.Avalonia.Dialogs.DialogService.Create(applicationLifetime, configureWindow: Maybe<Action<ConfigureWindowContext>>.From(ConfigureWindow));
        Plugins = AvailablePlugins();
        Clipboard = new ClipboardViewModel();
        TransferManager = new TransferManagerViewModel { AutoStartOnAdd = true };
    }

    public IClipboard Clipboard { get; set; }

    public NotificationService NotificationService { get; }

    private IPlugin[] Plugins { get; }

    private IDialogService DialogService { get; }

    public SyncSectionViewModel GetSyncViewModel()
    {
        return new SyncSectionViewModel(DialogService, Plugins, NotificationService, logger);
    }

    public SettingsSectionViewModel GetSettingsViewModel()
    {
        return new SettingsSectionViewModel(Plugins);
    }

    public SyncronizationSectionViewModel GetSynchronizationSection()
    {
        var collection = new[] { new FileSystemConnectionViewModel(new LocalFileSystemConnection("Local"), NotificationService, Clipboard, TransferManager) };
        var connections = new ReadOnlyObservableCollection<FileSystemConnectionViewModel>(new ObservableCollection<FileSystemConnectionViewModel>(collection));
        return new SyncronizationSectionViewModel(connections, DialogService);
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

    public ITransferManager TransferManager { get; }

    public ExplorerSectionViewModel GetExploreSection()
    {
        return new ExplorerSectionViewModel(NotificationService, Clipboard, TransferManager, logger);
    }
}