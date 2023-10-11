using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DynamicData;
using Serilog;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Comparer;
using Zafiro.Mixins;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.NewSync;

public class GranularSessionViewModel
{
    private readonly Subject<LongProgress> progress = new();

    public GranularSessionViewModel(IZafiroDirectory source, IZafiroDirectory destination, Maybe<ILogger> logger)
    {
        Source = source;
        Destination = destination;

        var sourceList = new SourceList<IFileActionViewModel>();
        var sourceListChanges = sourceList
            .Connect();

        sourceListChanges
            .Bind(out var actions)
            .Subscribe();

        var pendingSync = sourceListChanges
            .AutoRefresh()
            .Filter(x => x is { IsIgnored: false, IsSynced: false });

        ISubject<bool> canAnalyze = new Subject<bool>();
        Analyze = StoppableCommand.Create(() => Observable.FromAsync(() => new FileSystemComparer().Diff(source, destination)), Maybe.From(canAnalyze.AsObservable()));
        var canSync = pendingSync.ToCollection().Any();
        Actions = actions;
        SyncAll = StoppableCommand.Create(() =>
        {
            return Observable.FromAsync(async ct =>
            {
                var compositeAction = new CompositeAction(Actions.Where(x => !x.IsIgnored).Cast<IAction<LongProgress>>().ToList());
                using (compositeAction.Progress.Subscribe(progress))
                {
                    return await compositeAction.Execute(ct);
                }
            });
        }, Maybe.From(canSync));
        SyncAll.IsExecuting.Not().Subscribe(canAnalyze);
        ItemsUpdater(sourceList, Analyze.Results.Successes()).Subscribe();
    }

    public IObservable<LongProgress> Progress => progress.AsObservable();
    public StoppableCommand<Unit, Result> SyncAll { get; }
    public IZafiroDirectory Source { get; }
    public IZafiroDirectory Destination { get; }

    public ReadOnlyObservableCollection<IFileActionViewModel> Actions { get; }

    public StoppableCommand<Unit, Result<IEnumerable<FileDiff>>> Analyze { get; }
    public string Description => $"{Source} => {Destination}";

    private IObservable<IList<IFileActionViewModel>> ItemsUpdater(ISourceList<IFileActionViewModel> sourceList, IObservable<IEnumerable<FileDiff>> listsOfDiffs)
    {
        var observableOfLists = listsOfDiffs
            .SelectMany(diffs =>
                diffs
                    .ToObservable()
                    .Select(diff => Observable.FromAsync(() => GenerateAction(diff)))
                    .Merge(3)
                    .Successes().ToList());

        return observableOfLists.Do(list => sourceList.Edit(models =>
        {
            models.Clear();
            models.AddRange(list);
        }));
    }

    private IEnumerable<Task<Result<IFileActionViewModel>>> GenerateActions(IEnumerable<FileDiff> listsOfDiffs)
    {
        return listsOfDiffs.Select(GenerateAction);
    }

    private Task<Result<IFileActionViewModel>> GenerateAction(FileDiff fileDiff)
    {
        return fileDiff switch
        {
            Both both => Task.FromResult(Result.Success(new SkipFileActionViewModel(both))).Cast(model => (IFileActionViewModel)model),
            RightOnly rightOnly => Task.FromResult(Result.Success(new SkipFileActionViewModel(rightOnly))).Cast(model => (IFileActionViewModel)model),
            LeftOnly leftOnly => LeftOnlyFileActionViewModel.Create(leftOnly.Left.Path, Source, Destination).Cast(model => (IFileActionViewModel)model),
            _ => throw new ArgumentOutOfRangeException(nameof(fileDiff))
        };
    }
}