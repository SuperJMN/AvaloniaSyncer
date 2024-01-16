using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ByteSizeLib;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Actions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Actions;
using Zafiro.Reactive;

namespace AvaloniaSyncer.Sections.Synchronization.Actions;

public class CopyAction : ReactiveObject, IFileActionViewModel
{
    private readonly CopyFileAction copyAction;
    private readonly BehaviorSubject<bool> isSyncing = new(false);

    private CopyAction(CopyFileAction copyAction, Maybe<string> comment)
    {
        this.copyAction = copyAction;
        Progress = copyAction.Progress;
        Description = "Copy";
        Comment = comment.GetValueOrDefault("");
        LeftFile = Maybe<IZafiroFile>.From(this.copyAction.Source);
        RightFile = Maybe<IZafiroFile>.From(this.copyAction.Destination);
    }

    public string Comment { get; }
    public Maybe<IZafiroFile> LeftFile { get; }
    public Maybe<IZafiroFile> RightFile { get; }
    public string Description { get; }
    public bool IsIgnored => false;
    [Reactive] public bool IsSynced { get; private set; }
    public IObservable<LongProgress> Progress { get; }
    public IObservable<bool> IsSyncing => isSyncing.AsObservable();
    [Reactive] public string? Error { get; private set; }
    public IObservable<ByteSize> Rate => Progress.Select(x => x.Current).Rate().Select(ByteSize.FromBytes);

    public async Task<Result> Execute(CancellationToken cancellationToken)
    {
        isSyncing.OnNext(true);
        var execute = await copyAction.Execute(cancellationToken);
        isSyncing.OnNext(false);
        execute.TapError(e => Error = e);
        execute.Tap(() => IsSynced = true);
        return execute;
    }

    public static Task<Result<CopyAction>> Create(IZafiroFile source, IZafiroFile destination, Maybe<string> comment)
    {
        return CopyFileAction.Create(
                source: source,
                destination: destination,
                timeoutScheduler: Scheduler.Default,
                progressScheduler: Scheduler.Default,
                readTimeout: TimeSpan.FromSeconds(30))
            .Map(action => new CopyAction(action, comment));
    }
}