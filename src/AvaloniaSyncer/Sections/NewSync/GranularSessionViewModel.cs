using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography;
using CSharpFunctionalExtensions;
using DynamicData;
using Serilog;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Comparer;
using Zafiro.Mixins;
using Zafiro.ProgressReporting;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.NewSync;

public class GranularSessionViewModel
{
    private Subject<LongProgress> progress = new Subject<LongProgress>();

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

    private IObservable<IEnumerable<IFileActionViewModel>> ItemsUpdater(ISourceList<IFileActionViewModel> sourceList, IObservable<IEnumerable<FileDiff>> observable)
    {
        return observable
            .Select(GenerateActions)
            .Do(r => sourceList.Edit(list =>
            {
                list.Clear();
                list.AddRange(r);
            }));
    }

    private IEnumerable<IFileActionViewModel> GenerateActions(IEnumerable<FileDiff> listsOfDiffs)
    {
        return listsOfDiffs.Select(GenerateAction);
    }

    private IFileActionViewModel GenerateAction(FileDiff fileDiff)
    {
        return fileDiff switch
        {
            Both both => new SkipFileActionViewModel(both),
            RightOnly rightOnly => new SkipFileActionViewModel(rightOnly),
            LeftOnly leftOnly => new LeftOnlyFileActionViewModel(leftOnly.Left.Path, Source, Destination),
            _ => throw new ArgumentOutOfRangeException(nameof(fileDiff))
        };
    }
}