using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace AvaloniaSyncer.ViewModels;

public class SynchronizationViewModel : ViewModelBase
{
    public SynchronizationViewModel(string title, INotificationService notificationService, IZafiroDirectory origin, IZafiroDirectory dest)
    {
        Title = title;
        var syncer = new Syncer();
        var reactiveCommand = ReactiveCommand.CreateFromObservable(() =>
        {
            var observable = Observable
                .FromAsync(() => new FileSystemComparer().Diff(origin, dest))
                .Select(diffResult => diffResult.Map(diffs => syncer.Sync(origin, dest, diffs).ToEnumerable().ToList()));

            return observable;
        });
        GenerateSyncActions = reactiveCommand;

        reactiveCommand.Successes()
            .Select(list => list.Select(action => new SyncActionViewModel(notificationService, action)).ToList())
            .BindTo(this, model => model.SyncActions);

        reactiveCommand.Failures()
            .SelectMany(async message =>
            {
                await notificationService.Show(message);
                return Unit.Default;
            }).Subscribe();

        SyncAll = ReactiveCommand.CreateFromObservable(() => SyncActions!.Select(x => x.Sync.Execute()).Merge(3), this.WhenAnyValue(x => x.SyncActions, selector: list => list != null));
    }

    public ReactiveCommand<Unit, Result> SyncAll { get; set; }

    [Reactive] public List<SyncActionViewModel>? SyncActions { get; set; }

    public ReactiveCommand<Unit, Result<List<ISyncAction>>> GenerateSyncActions { get; }

    public string Title { get; }
}