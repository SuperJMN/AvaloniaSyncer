using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Connections;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.Wizard;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Synchronization;

public class SyncronizationSectionViewModel : ReactiveObject
{
    private readonly IClipboard clipboard;
    private readonly ReadOnlyObservableCollection<IFileSystemConnection> connections;
    private readonly IDialogService dialogService;
    private readonly INotificationService notificationService;
    private readonly ITransferManager transferManager;
    private readonly Maybe<ILogger> logger;
    private readonly ReadOnlyObservableCollection<GranularSessionViewModel> sessionsCollection;

    public SyncronizationSectionViewModel(IConnectionsRepository connectionsRepository, IDialogService dialogService, INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager, Maybe<ILogger> logger)
    {
        var sessions = new SourceList<GranularSessionViewModel>();
        connections = connectionsRepository.Connections;
        this.dialogService = dialogService;
        this.notificationService = notificationService;
        this.clipboard = clipboard;
        this.transferManager = transferManager;
        this.logger = logger;
        AddSession = ReactiveCommand.CreateFromTask(OnAddSession);
        AddSession.Values().Subscribe(session =>
        {
            sessions.Add(session);
            SelectedSession = session;
        });
        sessions.Connect().Bind(out sessionsCollection).Subscribe();
    }

    [Reactive]
    public GranularSessionViewModel SelectedSession { get; set; }

    public ReactiveCommand<Unit, Maybe<GranularSessionViewModel>> AddSession { get; set; }

    public ReadOnlyObservableCollection<GranularSessionViewModel> Sessions => sessionsCollection;

    public Task<Maybe<GranularSessionViewModel>> OnAddSession()
    {
        var wizard = CreateWizard();
        return ShowWizard(wizard);
    }

    private Wizard<DirectorySelectionViewModel, DirectorySelectionViewModel, GranularSessionViewModel> CreateWizard()
    {
        var wizard = new Wizard<DirectorySelectionViewModel, DirectorySelectionViewModel, GranularSessionViewModel>(
            new Page<DirectorySelectionViewModel>(new DirectorySelectionViewModel(connections, notificationService, clipboard, transferManager), "Next", "Choose the source directory"),
            new Page<DirectorySelectionViewModel>(new DirectorySelectionViewModel(connections, notificationService, clipboard, transferManager), "Finish", "Choose the destination directory"), 
            (p1, p2) => new GranularSessionViewModel(p1.CurrentDirectory, p2.CurrentDirectory, logger));
        return wizard;
    }

    private async Task<Maybe<GranularSessionViewModel>> ShowWizard(Wizard<DirectorySelectionViewModel, DirectorySelectionViewModel, GranularSessionViewModel> wizard)
    {
        var showDialog = await dialogService.ShowDialog<Wizard<DirectorySelectionViewModel, DirectorySelectionViewModel, GranularSessionViewModel>, GranularSessionViewModel>(wizard, "Synchronize folder", w => Observable.FromAsync(() => w.Result));
        return showDialog;
    }
}