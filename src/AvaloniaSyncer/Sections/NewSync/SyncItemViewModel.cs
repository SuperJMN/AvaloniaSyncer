using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.NewSync;

public class SyncItemViewModel : ViewModelBase
{
    private readonly ISyncAction syncAction;

    public SyncItemViewModel(ISyncAction syncAction)
    {
        var isSyncing = new Subject<bool>();
        this.syncAction = syncAction;

        Progress = this.syncAction.Progress;
        Stop = ReactiveCommand.Create(() => { }, isSyncing);

        Sync = ReactiveCommand.CreateFromObservable(() => Observable.Return(Unit.Default).Do(_ => Synced = false)
            .SelectMany(_ => Observable.FromAsync(ct => this.syncAction.Sync(ct)))
            .TakeUntil(Stop));

        Sync.IsExecuting.Subscribe(isSyncing);
        Sync.IsSuccess().BindTo(this, x => x.Synced);
        Sync.Failures().BindTo(this, x => x.Error);
    }

    public ReactiveCommand<Unit, Unit> Stop { get; set; }

    [Reactive] public string Error { get; private set; } = "";

    public IObservable<IProgress> Progress { get; }

    public string Source => syncAction switch
    {
        CopyAction copyAction => copyAction.Source.Path,
        SkipFileAction copyAction => copyAction.Source.Path,
        DeleteAction deleteAction => deleteAction.Source.Path,
        _ => ""
    };

    public string Destination => syncAction switch
    {
        CopyAction copyAction => copyAction.Destination.Path,
        SkipFileAction => "",
        _ => ""
    };

    public string Title => syncAction switch
    {
        CopyAction copyAction => "Copy origin to destination",
        DeleteAction deleteAction => "Delete destination",
        SkipFileAction deleteAction => "Skip",
        _ => ""
    };

    [Reactive] public bool Synced { get; set; }

    public ReactiveCommand<Unit, Result> Sync { get; }
    public IObservable<bool> IsSyncing => Sync.IsExecuting;
}