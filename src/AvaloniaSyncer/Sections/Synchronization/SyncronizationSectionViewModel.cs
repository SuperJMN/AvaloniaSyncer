﻿using System;
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
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.Wizard;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Synchronization;

public class SyncronizationSectionViewModel : ReactiveObject
{
    private readonly IClipboard clipboard;
    private readonly ReadOnlyObservableCollection<IZafiroFileSystemConnection> connections;
    private readonly IDialogService dialogService;
    private readonly INotificationService notificationService;
    private readonly ITransferManager transferManager;
    private readonly IContentOpener contentOpener;
    private readonly Maybe<ILogger> logger;
    private readonly ReadOnlyObservableCollection<ISyncSessionViewModel> sessionsCollection;

    public SyncronizationSectionViewModel(IConnectionsRepository connectionsRepository, IDialogService dialogService, INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager, IContentOpener contentOpener, Maybe<ILogger> logger)
    {
        var sessions = new SourceList<ISyncSessionViewModel>();
        connections = connectionsRepository.Connections;
        this.dialogService = dialogService;
        this.notificationService = notificationService;
        this.clipboard = clipboard;
        this.transferManager = transferManager;
        this.contentOpener = contentOpener;
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
    public ISyncSessionViewModel SelectedSession { get; set; }

    public ReactiveCommand<Unit, Maybe<ISyncSessionViewModel>> AddSession { get; set; }

    public ReadOnlyObservableCollection<ISyncSessionViewModel> Sessions => sessionsCollection;

    public Task<Maybe<ISyncSessionViewModel>> OnAddSession()
    {
        var wizard = CreateWizard();
        return ShowWizard(wizard);
    }

    private Wizard<DirectorySelectionViewModel, DirectorySelectionViewModel, ISyncSessionViewModel> CreateWizard()
    {
        var explorerContext = new ExplorerContext(notificationService, clipboard, transferManager, contentOpener);
        
        var wizard = new Wizard<DirectorySelectionViewModel, DirectorySelectionViewModel, ISyncSessionViewModel>(
            new Page<DirectorySelectionViewModel>(new DirectorySelectionViewModel(connections, explorerContext), "Next", "Choose the source directory"),
            new Page<DirectorySelectionViewModel>(new DirectorySelectionViewModel(connections, explorerContext), "Finish", "Choose the destination directory"), 
            (p1, p2) => new SyncSessionViewModel(p1.CurrentDirectory, p2.CurrentDirectory));
        return wizard;
    }

    private async Task<Maybe<ISyncSessionViewModel>> ShowWizard(Wizard<DirectorySelectionViewModel, DirectorySelectionViewModel, ISyncSessionViewModel> wizard)
    {
        var showDialog = await dialogService.ShowDialog<Wizard<DirectorySelectionViewModel, DirectorySelectionViewModel, ISyncSessionViewModel>, ISyncSessionViewModel>(wizard, "Synchronize folder", w => Observable.FromAsync(() => w.Result));
        return showDialog;
    }
}