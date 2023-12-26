using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteSizeLib;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Synchronization.Actions;

internal class DoNothing : ReactiveObject, IFileActionViewModel
{
    public DoNothing(string description)
    {
        Sync = StoppableCommand.Create(() => Observable.Return(Result.Success()), Maybe<IObservable<bool>>.None);
        IsSyncing = Observable.Return(false);
        Description = description;
    }

    public IObservable<bool> IsSyncing { get; }
    public string Error => "";
    public IObservable<ByteSize> Rate => Observable.Never<ByteSize>();
    public bool IsIgnored { get; } = true;
    public bool IsSynced { get; } = true;
    public string Description { get; }
    public IObservable<LongProgress> Progress => Observable.Never<LongProgress>();
    public Task<Result> Execute(CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Success());
    }

    public StoppableCommand<Unit, Result> Sync { get; }
}