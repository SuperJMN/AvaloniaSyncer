using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteSizeLib;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.FileSystem.Comparer;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Synchronization.Actions;

internal class SkipFileActionViewModel : ReactiveObject, IFileActionViewModel
{
    public SkipFileActionViewModel(FileDiff fileDiff)
    {
        FileDiff = fileDiff;
        Sync = StoppableCommand.Create(() => Observable.Return(Result.Success()), Maybe<IObservable<bool>>.None);
        IsSyncing = Observable.Return(false);
    }

    public FileDiff FileDiff { get; }

    public IObservable<bool> IsSyncing { get; }
    public string Error { get; }
    public IObservable<ByteSize> Rate => Observable.Never<ByteSize>();
    public bool IsIgnored { get; } = true;
    public bool IsSynced { get; } = true;
    public string Description => $"Skip {FileDiff}";
    public IObservable<LongProgress> Progress => Observable.Never<LongProgress>();
    public Task<Result> Execute(CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Success());
    }

    public StoppableCommand<Unit, Result> Sync { get; }
}