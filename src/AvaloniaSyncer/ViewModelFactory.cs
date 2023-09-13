using System;
using System.Collections.Generic;
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
using AvaloniaSyncer.Sections.Settings;
using AvaloniaSyncer.Sections.Synchronization.Sync;
using CSharpFunctionalExtensions;
using DynamicData;
using Microsoft.Build.Utilities;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.Pickers;
using Zafiro.Avalonia.Notifications;
using Zafiro.FileSystem.Local;
using IFileSystemConnection = AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization.IFileSystemConnection;

namespace AvaloniaSyncer;

public class ViewModelFactory
{
    public ViewModelFactory(IApplicationLifetime applicationLifetime, Visual control)
    {
        NotificationService = new NotificationService(new WindowNotificationManager(TopLevel.GetTopLevel(control)));
        DialogService = Zafiro.Avalonia.Dialogs.DialogService.Create(applicationLifetime, configureWindow: Maybe<Action<ConfigureWindowContext>>.From(ConfigureWindow));
        Plugins = AvailablePlugins();
    }

    public NotificationService NotificationService { get; }

    private IPlugin[] Plugins { get; }

    private IDialogService DialogService { get; }

    public SyncSectionViewModel GetSyncViewModel()
    {
        return new SyncSectionViewModel(DialogService, Plugins, NotificationService, Maybe.From(Log.Logger));
    }

    public SettingsSectionViewModel GetSettingsViewModel()
    {
        return new SettingsSectionViewModel(Plugins);
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
            new Plugin(fs => new FolderPicker(DialogService, fs, NotificationService), logger),
            new Plugins.SeaweedFS.Plugin(fs => new FolderPicker(DialogService, fs, NotificationService), logger),
            new Plugins.Sftp.Plugin(fs => new FolderPicker(DialogService, fs, NotificationService), logger)
        };
    }

    public ExplorerViewModel GetExploreSection()
    {
        var connectionRepo = new FileSystemConnectionRepository(Enumerable.Empty<IFileSystemConnection>());
        return new ExplorerViewModel();
    }
}