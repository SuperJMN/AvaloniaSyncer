using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteSizeLib;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Synchronization.Actions;

internal class DoNothing : ReactiveObject, IFileActionViewModel
{
    public DoNothing(string description, Maybe<string> comment, Maybe<IZafiroFile> leftFile, Maybe<IZafiroFile> rightFile)
    {
        Sync = StoppableCommand.Create(() => Observable.Return(Result.Success()), Maybe<IObservable<bool>>.None);
        IsSyncing = Observable.Return(false);
        Description = description;
        LeftFile = leftFile;
        RightFile = rightFile;
        Comment = comment.GetValueOrDefault("");
    }

    public StoppableCommand<Unit, Result> Sync { get; }
    public string Comment { get; }
    public Maybe<IZafiroFile> LeftFile { get; }
    public Maybe<IZafiroFile> RightFile { get; }
    public IObservable<bool> IsSyncing { get; }
    public string Error => "";
    public IObservable<ByteSize> Rate => Observable.Never<ByteSize>();
    public bool IsIgnored { get; } = true;
    public bool IsSynced { get; } = true;
    public string Description { get; }
    public IObservable<LongProgress> Progress => Observable.Never<LongProgress>();

    public Task<Result> Execute(CancellationToken cancellationToken) => Task.FromResult(Result.Success());
}