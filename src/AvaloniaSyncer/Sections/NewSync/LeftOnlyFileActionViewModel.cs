using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Actions;

namespace AvaloniaSyncer.Sections.NewSync;

internal class LeftOnlyFileActionViewModel : ReactiveObject, IFileActionViewModel
{
    private readonly CopyFileAction copyFileAction;
    private readonly BehaviorSubject<bool> isSyncing = new(false);

    public LeftOnlyFileActionViewModel(ZafiroPath left, IZafiroDirectory source, IZafiroDirectory destination, CopyFileAction copyFileAction)
    {
        this.copyFileAction = copyFileAction;
        Left = left;
        Source = source;
        Destination = destination;
        Progress = copyFileAction.Progress;
    }

    public ZafiroPath Left { get; }
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
        return execute;
    }

    public override string ToString()
    {
        return $"{nameof(Left)}: {Left}, {nameof(Source)}: {Source}, {nameof(Destination)}: {Destination}";
    }

    public static Task<Result<LeftOnlyFileActionViewModel>> Create(ZafiroPath left, IZafiroDirectory source, IZafiroDirectory destination)
    {
        return source.GetFromPath(left).CombineAndBind(destination.GetFromPath(left), (src, dst) =>
        {
            return CopyFileAction
                .Create(src, dst)
                .Map(action => new LeftOnlyFileActionViewModel(left, source, destination, action));
        });
    }
}