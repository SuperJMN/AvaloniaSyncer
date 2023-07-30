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
    public ISyncAction Action { get; }

    public SyncActionViewModel(INotificationService myNotificationService, ISyncAction action)
    {
        Action = action;
        Progress = action.Progress;
        Sync = ReactiveCommand.CreateFromObservable(() => Observable.Return(Unit.Default).Do(_ => Synced = false).SelectMany(_ => action.Sync()));

        Sync.IsSuccess().BindTo(this, x => x.Synced);
        Sync.HandleErrorsWith(myNotificationService);
    }

    public IObservable<RelativeProgress<long>> Progress { get; }

    public string Source => Action switch
    {
        CopyAction copyAction => copyAction.Source.Path,
        DeleteAction deleteAction => deleteAction.Source.Path,
        _ => ""
    };
    public string? Destination => Action switch
    {
        CopyAction copyAction => copyAction.Source.Path,
        _ => ""
    };

    [Reactive]
    public bool Synced { get; set; }

    public ReactiveCommand<Unit, Result> Sync { get; }
}