using System;
using System.Reactive;
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
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.NewSync;

internal class LeftOnlyFileActionViewModel : ReactiveObject, IFileActionViewModel
{
    private readonly BehaviorSubject<LongProgress> progress = new(new LongProgress());

    public LeftOnlyFileActionViewModel(ZafiroPath left, IZafiroDirectory source, IZafiroDirectory destination)
    {
        Left = left;
        Source = source;
        Destination = destination;
        Sync = StoppableCommand.Create(() => Observable.FromAsync(ct => OnSync(source, destination, ct)), Maybe<IObservable<bool>>.None);
        IsSyncing = Sync.IsExecuting;
        Sync.Results.Successes().Do(_ => IsSynced = true).Subscribe();
    }

    public ZafiroPath Left { get; }
    public IZafiroDirectory Source { get; }
    public IZafiroDirectory Destination { get; }

    public IObservable<bool> IsSyncing { get; }

    public bool IsIgnored { get; } = false;

    [Reactive]
    public bool IsSynced { get; private set;  }

    public StoppableCommand<Unit, Result> Sync { get; }

    public string Description => $"Copy {Left}";
    public IObservable<LongProgress> Progress => progress.AsObservable();

    public Task<Result> Execute(CancellationToken cancellationToken)
    {
        var taskCompletionSource = new TaskCompletionSource<Result>();
        Sync.Start.Execute().Subscribe(result => taskCompletionSource.SetResult(result), cancellationToken);
        return taskCompletionSource.Task;
    }
    
    public override string ToString()
    {
        return $"{nameof(Left)}: {Left}, {nameof(Source)}: {Source}, {nameof(Destination)}: {Destination}";
    }

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
}