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
using Zafiro.Reactive;

namespace AvaloniaSyncer.Sections.Synchronization.Actions;

public class DeleteDestinationAction : ReactiveObject, IFileActionViewModel
{
    private readonly DeleteFileAction deleteFileAction;
    private readonly BehaviorSubject<bool> isSyncing = new(false);

    private DeleteDestinationAction(DeleteFileAction deleteFileAction, Maybe<string> comment)
    {
        this.deleteFileAction = deleteFileAction;
        Progress = deleteFileAction.Progress;
        Description = "Delete";
        Comment = comment.GetValueOrDefault("");
        LeftFile = Maybe<IZafiroFile>.None;
        RightFile = Maybe<IZafiroFile>.From(this.deleteFileAction.Source);
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
        var execute = await deleteFileAction.Execute(cancellationToken);
        isSyncing.OnNext(false);
        execute.TapError(e => Error = e);
        execute.Tap(() => IsSynced = true);
        return execute;
    }

    public static async Task<Result<DeleteDestinationAction>> Create(IZafiroFile source, Maybe<string> comment)
    {
        return DeleteFileAction.Create(source).Map(action => new DeleteDestinationAction(action, comment));
    }
}