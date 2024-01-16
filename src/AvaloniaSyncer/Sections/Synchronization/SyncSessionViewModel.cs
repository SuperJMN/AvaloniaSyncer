using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Synchronization.Actions;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Comparer;
using Zafiro.Reactive;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Synchronization;

public class SyncSessionViewModel : ReactiveObject, ISyncSessionViewModel
{
    private readonly ObservableAsPropertyHelper<IEnumerable<IFileActionViewModel>> pendingSyncActions;
    private readonly Subject<LongProgress> progress = new();
    private readonly ObservableAsPropertyHelper<IEnumerable<IFileActionViewModel>> syncActions;

    public SyncSessionViewModel(IZafiroDirectory source, IZafiroDirectory destination)
    {
        Source = source;
        Destination = destination;
        ISubject<bool> canAnalyze = new Subject<bool>();
        Analyze = AnalyzeCommand(canAnalyze);
        syncActions = Analyze.Start.Successes().ToProperty(this, x => x.SyncActions);
        var canSync = this.WhenAnyValue(x => x.SyncActions, x => x != null && x.Any()).CombineLatest(canAnalyze, (hasSyncActions, isAnalyzing) => hasSyncActions || !isAnalyzing);
        Sync = DoSync(canSync);
        Sync.IsExecuting.Not().Subscribe(canAnalyze);
        pendingSyncActions = this.WhenAnyValue(model => model.SyncActions, x => x == null ? [] : x.Where(i => i.ShouldSync())).ToProperty(this, x => x.PendingSyncActions);
        IsAnalyzing = Analyze.IsExecuting;
        IsSyncing = Sync.IsExecuting;
    }

    public IZafiroDirectory Source { get; }
    public IZafiroDirectory Destination { get; }

    public IEnumerable<IFileActionViewModel> PendingSyncActions => pendingSyncActions.Value;
    public IObservable<LongProgress> Progress => progress.AsObservable();

    public IObservable<bool> IsAnalyzing { get; }

    public string Description => $"{Source} => {Destination}";

    public IObservable<bool> IsSyncing { get; }

    public IStoppableCommand<Unit, Result> Sync { get; }

    public IEnumerable<IFileActionViewModel> SyncActions => syncActions.Value;

    public IStoppableCommand<Unit, Result<IEnumerable<IFileActionViewModel>>> Analyze { get; }

    private IStoppableCommand<Unit, Result> DoSync(IObservable<bool> canSync) => StoppableCommand.CreateFromTask(SyncTask, Maybe<IObservable<bool>>.From(canSync));

    private async Task<Result> SyncTask(CancellationToken ct)
    {
        var compositeAction = new CompositeAction(PendingSyncActions.Cast<IAction<LongProgress>>().ToList());
        Result result;
        using (compositeAction.Progress.Subscribe(progress))
        {
            result = await compositeAction.Execute(ct);
        }

        return result;
    }

    private IStoppableCommand<Unit, Result<IEnumerable<IFileActionViewModel>>> AnalyzeCommand(IObservable<bool> canAnalyze)
    {
        return StoppableCommand.CreateFromTask(_ => { return Async.Await(() => new FileSystemComparer().Diff(Source, Destination)).Bind(ConvertMe); }, Maybe.From(canAnalyze));
    }

    private async Task<Result<IEnumerable<IFileActionViewModel>>> ConvertMe(IEnumerable<FileDiff> diffs)
    {
        var taskObs = diffs.Select(GenerateActionFor);
        var result = await Task.WhenAll(taskObs);
        var combined = result.Combine();

        return combined;
    }

    private Task<Result<IFileActionViewModel>> GenerateActionFor(FileDiff fileDiff) => new FileActionFactory(Source, Destination).Create(fileDiff, new SizeCompareStrategy());
}