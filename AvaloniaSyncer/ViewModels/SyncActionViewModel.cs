using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.FileSystem;
using Zafiro.Functional;
using Zafiro.ProgressReporting;
using Zafiro.UI;

namespace AvaloniaSyncer.ViewModels;

public class SyncActionViewModel : ViewModelBase
{
    private readonly ISyncAction syncAction;

    public SyncActionViewModel(INotificationService myNotificationService, ISyncAction syncAction)
    {
        this.syncAction = syncAction;
        Progress = this.syncAction.Progress;
        Sync = ReactiveCommand.CreateFromObservable(() => Observable.Return(Unit.Default).Do(_ => Synced = false).SelectMany(_ => this.syncAction.Sync()));

        Sync.IsSuccess().BindTo(this, x => x.Synced);
        Sync.HandleErrorsWith(myNotificationService);
    }

    public IObservable<RelativeProgress<long>> Progress { get; }

    public string Source => syncAction switch
    {
        CopyAction copyAction => copyAction.Source.Path,
        DeleteAction deleteAction => deleteAction.Source.Path,
        _ => ""
    };

    public string Destination => syncAction switch
    {
        CopyAction copyAction => copyAction.Destination.Path,
        _ => ""
    };

    [Reactive] public bool Synced { get; set; }

    public ReactiveCommand<Unit, Result> Sync { get; }
}