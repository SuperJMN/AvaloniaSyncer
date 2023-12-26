using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Synchronization.Actions;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using Serilog;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Comparer;
using Zafiro.Mixins;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Synchronization;

public class GranularSessionViewModel
{
    private readonly Subject<LongProgress> progress = new();

    public GranularSessionViewModel(IZafiroDirectory source, IZafiroDirectory destination, Maybe<ILogger> logger)
    {
        Source = source;
        Destination = destination;

        var syncActionsList = new SourceList<IFileActionViewModel>();
        var sourceListChanges = syncActionsList
            .Connect();

        sourceListChanges
            .Bind(out var syncActions)
            .Subscribe();

        SyncActions = syncActions;

        var pendingSync = sourceListChanges
            .AutoRefresh()
            .Filter(x => x is { IsIgnored: false, IsSynced: false });

        ISubject<bool> canAnalyze = new Subject<bool>();
        Analyze = StoppableCommand.Create(() => Observable.FromAsync(() => new FileSystemComparer2().Diff(source, destination)), Maybe.From(canAnalyze.AsObservable()));
        var canSync = pendingSync.ToCollection().Any();

        SyncAll = StoppableCommand.Create(() =>
        {
            return Observable.FromAsync(async ct =>
            {
                var compositeAction = new CompositeAction(SyncActions.Where(x => !x.IsIgnored).Cast<IAction<LongProgress>>().ToList());
                using (compositeAction.Progress.Subscribe(progress))
                {
                    return await compositeAction.Execute(ct);
                }
            });
        }, Maybe.From(canSync));
        SyncAll.IsExecuting.Not().Subscribe(canAnalyze);
        ItemsUpdater(syncActionsList, Analyze.Start.Successes()).Subscribe();
        IsSyncing = SyncAll.IsExecuting;
    }

    public IObservable<LongProgress> Progress => progress.AsObservable();
    public StoppableCommand<Unit, Result> SyncAll { get; }
    public IZafiroDirectory Source { get; }
    public IZafiroDirectory Destination { get; }
    public ReadOnlyObservableCollection<IFileActionViewModel> SyncActions { get; }
    public StoppableCommand<Unit, Result<IEnumerable<FileDiff>>> Analyze { get; }
    public string Description => $"{Source} => {Destination}";
    public IObservable<bool> IsSyncing { get; }

    private IObservable<IList<IFileActionViewModel>> ItemsUpdater(ISourceList<IFileActionViewModel> actions, IObservable<IEnumerable<FileDiff>> listsOfDiffs)
    {
        var observableOfLists = listsOfDiffs
            .SelectMany(diffs =>
                diffs
                    .ToObservable()
                    .Select(diff => Observable.FromAsync(() => GenerateActionFor(diff)))
                    .Merge(3)
                    .Successes().ToList());

        return observableOfLists
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(acts => actions.EditDiff(acts));
    }

    private Task<Result<IFileActionViewModel>> GenerateActionFor(FileDiff fileDiff)
    {
        return fileDiff switch
        {
            BothDiff both => Task.FromResult(Result.Success(new SkipFileActionViewModel(both))).Cast(model => (IFileActionViewModel) model),
            RightOnlyDiff rightOnly => Task.FromResult(Result.Success(new SkipFileActionViewModel(rightOnly))).Cast(model => (IFileActionViewModel) model),
            LeftOnlyDiff leftOnly => LeftOnlyFileActionViewModel.Create(leftOnly.Left, Destination).Cast(model => (IFileActionViewModel) model),
            _ => throw new ArgumentOutOfRangeException(nameof(fileDiff))
        };
    }
}