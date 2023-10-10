using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Comparer;
using Zafiro.FileSystem.Synchronizer;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.NewSync;

public class GranularSessionViewModel
{
    public IZafiroDirectory Source { get; }
    public IZafiroDirectory Destination { get; }

    public GranularSessionViewModel(IZafiroDirectory source, IZafiroDirectory destination, Maybe<ILogger> logger)
    {
        Source = source;
        Destination = destination;
        Analyze = StoppableCommand.Create(() => Observable.FromAsync(() => new FileSystemComparer().Diff(source, destination)), Maybe<IObservable<bool>>.None);
        Diff = Analyze.Results.Successes();
        Actions = GenerateActions(Diff);
    }

    public IObservable<IEnumerable<IFileActionViewModel>> Actions { get; set; }

    private IObservable<IEnumerable<IFileActionViewModel>> GenerateActions(IObservable<IEnumerable<FileDiff>> listsOfDiffs)
    {
        return listsOfDiffs.Select(diffs => diffs.Select(diff => GenerateAction(diff)));
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
    public IObservable<IEnumerable<FileDiff>> Diff { get; }

    public StoppableCommand<Unit, Result<IEnumerable<FileDiff>>> Analyze { get; }
    public string Description => $"{Source} => {Destination}";
}

internal class SkipFileActionViewModel : IFileActionViewModel
{
    public FileDiff FileDiff { get; }

    public SkipFileActionViewModel(FileDiff fileDiff)
    {
        FileDiff = fileDiff;
        Sync = StoppableCommand.Create(() => Observable.Return(Result.Success()), Maybe<IObservable<bool>>.None);
        IsSyncing = Sync.IsExecuting;
    }

    public IObservable<bool> IsSyncing { get; }
    public string Description => $"Skip {FileDiff}";
    public IObservable<LongProgress> Progress => Observable.Never<LongProgress>();
    public StoppableCommand<Unit, Result> Sync { get; }
}

internal class LeftOnlyFileActionViewModel : IFileActionViewModel
{
    private readonly BehaviorSubject<LongProgress> progress = new(new LongProgress());
    public ZafiroPath Left { get; }
    public IZafiroDirectory Source { get; }
    public IZafiroDirectory Destination { get; }

    public LeftOnlyFileActionViewModel(ZafiroPath left, IZafiroDirectory source, IZafiroDirectory destination)
    {
        Left = left;
        Source = source;
        Destination = destination;
        Sync = StoppableCommand.Create(() => Observable.FromAsync(ct => OnSync(source, destination, ct)), Maybe<IObservable<bool>>.None);
        IsSyncing = Sync.IsExecuting;
    }

    public IObservable<bool> IsSyncing { get; }

    private Task<Result> OnSync(IZafiroDirectory source, IZafiroDirectory destination, CancellationToken ct)
    {
        return source.GetFromPath(Left).CombineAndBind(destination.GetFromPath(Left), (src, dst) =>
        {
            return CopyFileAction
                .Create(src, dst)
                .Bind(async action =>
                {
                    using (action.Progress.Subscribe(progress))
                    {
                        return await action.Execute(ct);
                    }
                });
        });
    }

    public StoppableCommand<Unit, Result> Sync { get; }


    public override string ToString()
    {
        return $"{nameof(Left)}: {Left}, {nameof(Source)}: {Source}, {nameof(Destination)}: {Destination}";
    }

    public string Description => $"Copy {Left}";
    public IObservable<LongProgress> Progress => progress.AsObservable();
}