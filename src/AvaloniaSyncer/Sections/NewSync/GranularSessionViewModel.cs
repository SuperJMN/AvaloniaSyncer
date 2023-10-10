using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using DynamicData;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Comparer;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.NewSync;

public class GranularSessionViewModel
{
    public GranularSessionViewModel(IZafiroDirectory source, IZafiroDirectory destination, Maybe<ILogger> logger)
    {
        var sourceList = new SourceList<IFileActionViewModel>();
        Source = source;
        Destination = destination;

        ISubject<bool> canSync = new Subject<bool>();
        Analyze = StoppableCommand.Create(() => Observable.FromAsync(() => new FileSystemComparer().Diff(source, destination)), Maybe.From(canSync.AsObservable()));

        ItemsUpdater(sourceList).Subscribe();

        sourceList
            .Connect()
            .Bind(out var actions);

        Actions = actions;
    }

    public IZafiroDirectory Source { get; }
    public IZafiroDirectory Destination { get; }

    public ReadOnlyObservableCollection<IFileActionViewModel> Actions { get; }

    public StoppableCommand<Unit, Result<IEnumerable<FileDiff>>> Analyze { get; }
    public string Description => $"{Source} => {Destination}";

    private IObservable<IEnumerable<IFileActionViewModel>> ItemsUpdater(ISourceList<IFileActionViewModel> sourceList)
    {
        return Analyze.Results.Successes()
            .Select(GenerateActions)
            .Do(r => sourceList.Edit(list =>
            {
                list.Clear();
                list.AddRange(r);
            }));
    }

    private IObservable<IEnumerable<IFileActionViewModel>> GenerateActions(IObservable<IEnumerable<FileDiff>> listsOfDiffs)
    {
        return listsOfDiffs.Select(diffs => diffs.Select(diff => GenerateAction(diff)));
    }

    private IEnumerable<IFileActionViewModel> GenerateActions(IEnumerable<FileDiff> listsOfDiffs)
    {
        return listsOfDiffs.Select(diff => GenerateAction(diff));
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