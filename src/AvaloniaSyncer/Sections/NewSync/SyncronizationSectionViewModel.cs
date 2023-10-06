using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using Zafiro.Avalonia.Controls;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.Wizard;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.NewSync;

public class SyncronizationSectionViewModel : ReactiveObject
{
    private readonly IClipboard clipboard;
    private readonly ReadOnlyObservableCollection<IFileSystemConnection> connections;
    private readonly IDialogService dialogService;
    private readonly INotificationService notificationService;
    private readonly ITransferManager transferManager;
    private readonly ReadOnlyObservableCollection<SessionViewModel> sessionsCollection;

    public SyncronizationSectionViewModel(ReadOnlyObservableCollection<IFileSystemConnection> connections, IDialogService dialogService, INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager)
    {
        var sessions = new SourceList<SessionViewModel>();
        this.connections = connections;
        this.dialogService = dialogService;
        this.notificationService = notificationService;
        this.clipboard = clipboard;
        this.transferManager = transferManager;
        AddSession = ReactiveCommand.CreateFromTask(OnAddSession);
        AddSession.Values().Subscribe(session => sessions.Add(session));
        sessions.Connect().Bind(out sessionsCollection).Subscribe();
    }

    public ReactiveCommand<Unit, Maybe<SessionViewModel>> AddSession { get; set; }

    public ReadOnlyObservableCollection<SessionViewModel> Sessions => sessionsCollection;

    public Task<Maybe<SessionViewModel>> OnAddSession()
    {
        var wizard = CreateWizard();
        return ShowWizard(wizard);
    }

    private Wizard<DirectorySelectionViewModel, DirectorySelectionViewModel, SessionViewModel> CreateWizard()
    {
        var wizard = new Wizard<DirectorySelectionViewModel, DirectorySelectionViewModel, SessionViewModel>(
            new Page<DirectorySelectionViewModel>(new DirectorySelectionViewModel(connections, notificationService, clipboard, transferManager), "Next"),
            new Page<DirectorySelectionViewModel>(new DirectorySelectionViewModel(connections, notificationService, clipboard, transferManager), "Finish"), (p1, p2) => new SessionViewModel(p1.CurrentDirectory, p2.CurrentDirectory));
        return wizard;
    }

    private async Task<Maybe<SessionViewModel>> ShowWizard(Wizard<DirectorySelectionViewModel, DirectorySelectionViewModel, SessionViewModel> wizard)
    {
        var showDialog = await dialogService.ShowDialog<Wizard<DirectorySelectionViewModel, DirectorySelectionViewModel, SessionViewModel>, SessionViewModel>(wizard, "Do something, boi", w => Observable.FromAsync(() => w.Result));
        return showDialog;
    }
}