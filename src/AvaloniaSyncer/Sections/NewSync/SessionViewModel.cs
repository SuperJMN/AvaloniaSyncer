using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AvaloniaSyncer.Sections.Synchronization.Sync;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using Zafiro.Avalonia.WizardOld.Interfaces;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.Mixins;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.NewSync;

public class SessionViewModel : ReactiveValidationObject, IValidatable
{
    private readonly IZafiroDirectory source;
    private readonly IZafiroDirectory destination;

    public SessionViewModel(INotificationService notificationService, IZafiroDirectory source, IZafiroDirectory destination, Maybe<ILogger> logger)
    {
        this.source = source;
        this.destination = destination;
        var isAnalyzing = new Subject<bool>();

        var canSync = this.WhenAnyValue(x => x.SyncActions).NotNull().CombineLatest(isAnalyzing, (containsActions, isSyncing) => containsActions && !isSyncing);
        SyncAll = StoppableCommand.Create(OnSyncAll, canSync);

        Analyze = StoppableCommand.Create(() => OnAnalize(source, destination, logger), SyncAll.IsExecuting.Not());
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

    public string Description => $"{source} => {destination}";

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