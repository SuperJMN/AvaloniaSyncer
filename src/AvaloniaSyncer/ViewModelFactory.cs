using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using AvaloniaSyncer.Sections.Connections;
using AvaloniaSyncer.Sections.Explorer;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using AvaloniaSyncer.Sections.Synchronization;
using AvaloniaSyncer.Sections.Syncronization;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Serilog;
using Zafiro;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.Clipboard;
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
        Clipboard = new ClipboardViewModel();
        TransferManager = new TransferManagerViewModel { AutoStartOnAdd = true };
        ConnectionsRepository = GetConnectionsRepository(Logger);
    }

    private IObservable<IConnectionsRepository> ConnectionsRepository { get; }

    private Maybe<ILogger> Logger { get; }

    private IClipboard Clipboard { get; }

    private NotificationService NotificationService { get; }

    private IDialogService DialogService { get; }

    private ITransferManager TransferManager { get; }

    public async Task<SyncronizationSectionViewModel> GetSynchronizationSection()
    {
        return new SyncronizationSectionViewModel(
            await ConnectionsRepository,
            DialogService,
            NotificationService,
            Clipboard,
            TransferManager, Logger);
    }

    public ExplorerSectionViewModel GetExploreSection()
    {
        return new ExplorerSectionViewModel(NotificationService, Clipboard, TransferManager, Logger);
    }

    public async Task<ConnectionsSectionViewModel> GetConnectionsViewModel()
    {
        try
        {
            var connectionsRepository = await ConnectionsRepository;
            return new ConnectionsSectionViewModel(connectionsRepository, NotificationService, DialogService);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static void ConfigureWindow(ConfigureWindowContext context)
    {
        context.ToConfigure.Width = context.Parent.Bounds.Width / 1.5;
        context.ToConfigure.Height = context.Parent.Bounds.Height / 1.5;
    }

    private IObservable<IConnectionsRepository> GetConnectionsRepository(Maybe<ILogger> logger)
    {
        return Observable.FromAsync(async () =>
            {
                var store = new ConfigurationStore(() => File.OpenRead("Connections.json"), () => File.OpenWrite("Connections.json"));
                var loadResult = await store.Load();
                var result = loadResult.Map(enumerable => new ConnectionsRepository(enumerable.Select(x => Mapper.ToSystem(x, logger)), logger));
                var repo = result.GetValueOrDefault(() => new ConnectionsRepository(Enumerable.Empty<IFileSystemConnection>(), logger));
                return repo;
            }).Replay()
            .RefCount();
    }
}