using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using AvaloniaSyncer.Sections.Connections;
using AvaloniaSyncer.Sections.Explorer;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using AvaloniaSyncer.Sections.Synchronization;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.Notifications;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace AvaloniaSyncer;

public class ViewModelFactory
{
    public ViewModelFactory(IApplicationLifetime applicationLifetime, Visual control, IContentOpener contentOpener, Maybe<ILogger> logger)
    {
        Logger = logger;
        NotificationService = new NotificationService(new WindowNotificationManager(TopLevel.GetTopLevel(control)));
        DialogService = Zafiro.Avalonia.Dialogs.DialogService.Create(applicationLifetime, configureWindow: Maybe<Action<ConfigureWindowContext>>.From(ConfigureWindow));
        Clipboard = new ClipboardViewModel();
        TransferManager = new TransferManagerViewModel { AutoStartOnAdd = true };
        ConnectionsRepository = GetConnectionsRepository(Logger);
        ContentOpener = contentOpener;
    }

    public IContentOpener ContentOpener { get; }

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

    public async Task<ExplorerSectionViewModel> GetExploreSection()
    {
        return new ExplorerSectionViewModel(await ConnectionsRepository, NotificationService, Clipboard, TransferManager, Logger, ContentOpener);
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
        context.ToConfigure.MinWidth = 650;
        context.ToConfigure.MinHeight = 400;
        context.ToConfigure.Width = context.Parent.Bounds.Width / 1.5;
        context.ToConfigure.Height = context.Parent.Bounds.Height / 1.5;
    }

    private IObservable<IConnectionsRepository> GetConnectionsRepository(Maybe<ILogger> logger)
    {
        return Observable.FromAsync(async () =>
            {
                var store = new ConfigurationStore(() => ApplicationStorage.OpenRead("Connections"), () => ApplicationStorage.OpenWrite("Connections"));
                var loadResult = await store.Load();
                var result = loadResult.Map(enumerable => new ConnectionsRepository(enumerable.Select(x => Mapper.ToSystem(x, logger)), logger, store));
                var repo = result.GetValueOrDefault(() => new ConnectionsRepository(Enumerable.Empty<IZafiroFileSystemConnection>(), logger, store));
                return repo;
            })
            .Replay()
            .RefCount();
    }
}