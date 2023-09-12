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

        var isAnalyzing = new Subject<bool>();

        var canSync = this.WhenAnyValue(x => x.SyncActions).NotNull().CombineLatest(isAnalyzing, (containsActions, isSyncing) => containsActions && !isSyncing);
        SyncAll = Stoppable.Create(OnSyncAll, canSync);

        Analyze = Stoppable.Create(() => OnAnalize(origin, dest, logger), SyncAll.IsExecuting.Not());
        Analyze.IsExecuting.Subscribe(isAnalyzing);

        Analyze.Results.Successes()
            .Select(list => list.Select(action => new SyncItemViewModel(action)).ToList())
            .BindTo(this, model => model.SyncActions);

        Analyze.Results.Failures().Merge(SyncAll.Results.Failures())
            .SelectMany(async message =>
            {
                await notificationService.Show(message, "Something went wrong 😅");
                return Unit.Default;
            }).Subscribe();

        IsBusy = SyncAll.IsExecuting.Merge(SyncAll.IsExecuting);
    }

    public StoppableCommand<Unit, Result<List<ISyncAction>>> Analyze { get; }

    public StoppableCommand<Unit, Result> SyncAll { get; }

    [Reactive] public bool SkipIdentical { get; set; } = true;
    [Reactive] public bool DeleteNonExistent { get; set; } = false;
    [Reactive] public bool CanOverwrite { get; set; } = false;
    [Reactive] public List<SyncItemViewModel>? SyncActions { get; set; }

    public string Title { get; }
    public IObservable<bool> IsBusy { get; }
    public IObservable<bool> IsValid => this.IsValid();

    private IObservable<Result<List<ISyncAction>>> OnAnalize(IZafiroDirectory origin, IZafiroDirectory dest, Maybe<ILogger> logger)
    {
        var observable = Observable
            .FromAsync(() => new FileSystemComparer().Diff(origin, dest))
            .Select(diffResult =>
            {
                var syncer = new Syncer(logger)
                {
                    SkipIdentical = SkipIdentical,
                    CanOverwrite = CanOverwrite,
                    DeleteNonExistent = DeleteNonExistent
                };
                return diffResult.Map(diffs => syncer.Sync(origin, dest, diffs).ToEnumerable().ToList());
            });

        return observable;
    }

    private IObservable<Result> OnSyncAll()
    {
        return SyncActions!
            .Where(x => !x.Synced)
            .Select(x => x.Sync.Execute())
            .Merge(3);
    }
}