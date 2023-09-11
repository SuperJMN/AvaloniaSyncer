using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using Zafiro.Avalonia.Wizard.Interfaces;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.Mixins;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Synchronization.Sync;

public class SynchronizationViewModel : ReactiveValidationObject, IValidatable
{
    public SynchronizationViewModel(string title, INotificationService notificationService, IZafiroDirectory origin, IZafiroDirectory dest, Maybe<ILogger> logger)
    {
        Title = title;

        var isSyncing = new Subject<bool>();
        var isAnalyzing = new Subject<bool>();
        Stop = ReactiveCommand.Create(() => { }, isSyncing);
        var canSync = this.WhenAnyValue(x => x.SyncActions).NotNull().CombineLatest(isAnalyzing, (containsActions, isSyncing) => containsActions && !isSyncing);
        SyncAll = ReactiveCommand.CreateFromObservable(OnSyncAll, canSync);
        SyncAll.IsExecuting.Subscribe(isSyncing);

        var reactiveCommand = ReactiveCommand.CreateFromObservable(() =>
        {
            var observable = Observable
                .FromAsync(() => new FileSystemComparer().Diff(origin, dest))
                .Select(diffResult =>
                {
                    var syncer = new Syncer(logger) { SkipIdentical = SkipIdentical };
                    return diffResult.Map(diffs => syncer.Sync(origin, dest, diffs).ToEnumerable().ToList());
                });

            return observable;
        }, SyncAll.IsExecuting.Not());

        GenerateSyncActions = reactiveCommand;
        GenerateSyncActions.IsExecuting.Subscribe(isAnalyzing);

        reactiveCommand.Successes()
            .Select(list => list.Select(action => new SyncItemViewModel(action)).ToList())
            .BindTo(this, model => model.SyncActions);

        reactiveCommand.Failures()
            .SelectMany(async message =>
            {
                await notificationService.Show(message);
                return Unit.Default;
            }).Subscribe();

        IsBusy = GenerateSyncActions.IsExecuting.Merge(SyncAll.IsExecuting);
        IsSyncing = SyncAll.IsExecuting;
    }

    public IObservable<bool> IsSyncing { get; }

    public ReactiveCommand<Unit, Unit> Stop { get; }

    [Reactive] public bool SkipIdentical { get; set; } = true;

    public ReactiveCommand<Unit, Result> SyncAll { get; set; }

    [Reactive] public List<SyncItemViewModel>? SyncActions { get; set; }

    public ReactiveCommand<Unit, Result<List<ISyncAction>>> GenerateSyncActions { get; }

    public string Title { get; }
    public IObservable<bool> IsBusy { get; }

    public IObservable<bool> IsValid => this.IsValid();

    private IObservable<Result> OnSyncAll()
    {
        return SyncActions!
            .Where(x => !x.Synced)
            .Select(x => x.Sync.Execute())
            .Merge(3)
            .TakeUntil(Stop);
    }
}