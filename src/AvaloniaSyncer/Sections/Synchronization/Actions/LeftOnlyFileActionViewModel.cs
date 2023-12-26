using System;
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
using Zafiro.FileSystem.Comparer;
using Zafiro.Mixins;

namespace AvaloniaSyncer.Sections.Synchronization.Actions;

internal class LeftOnlyFileActionViewModel : ReactiveObject, IFileActionViewModel
{
    private readonly CopyFileAction copyFileAction;
    private readonly BehaviorSubject<bool> isSyncing = new(false);

    public LeftOnlyFileActionViewModel(IZafiroFile left, IZafiroDirectory destination, CopyFileAction copyFileAction)
    {
        this.copyFileAction = copyFileAction;
        Left = left;
        Source = left.Parent();
        Destination = destination;
        Progress = copyFileAction.Progress;
    }

    public IZafiroFile Left { get; }
    public IZafiroDirectory Source { get; }
    public IZafiroDirectory Destination { get; }

    public bool IsIgnored { get; } = false;

    [Reactive] public bool IsSynced { get; private set; }

    public string Description => $"[Copy] {Left}";
    public IObservable<LongProgress> Progress { get; }
    public IObservable<bool> IsSyncing => isSyncing.AsObservable();

    public async Task<Result> Execute(CancellationToken cancellationToken)
    {
        isSyncing.OnNext(true);
        var execute = await copyFileAction.Execute(cancellationToken);
        isSyncing.OnNext(false);
        execute.TapError(e => Error = e);
        execute.Tap(() => IsSynced = true);
        return execute;
    }

    [Reactive]
    public string? Error { get; private set; }

    public IObservable<ByteSize> Rate => Progress.Select(x => x.Current).Rate().Select(ByteSize.FromBytes);

    public override string ToString()
    {
        return $"{nameof(Left)}: {Left}, {nameof(Source)}: {Source}, {nameof(Destination)}: {Destination}";
    }

    public static Task<Result<LeftOnlyFileActionViewModel>> Create(FileWithMetadata left, IZafiroDirectory destination)
    {
        return CopyFileAction.Create(left.File, left.File.EquivaletIn(destination)).Map(action => new LeftOnlyFileActionViewModel(left.File, destination, action));
    }
}