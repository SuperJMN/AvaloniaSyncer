using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.FileSystem.Comparer;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.NewSync;

internal class SkipFileActionViewModel : IFileActionViewModel
{
    public SkipFileActionViewModel(FileDiff fileDiff)
    {
        FileDiff = fileDiff;
        Sync = StoppableCommand.Create(() => Observable.Return(Result.Success()), Maybe<IObservable<bool>>.None);
        IsSyncing = Sync.IsExecuting;
    }

    public FileDiff FileDiff { get; }

    public IObservable<bool> IsSyncing { get; }
    public string Description => $"Skip {FileDiff}";
    public IObservable<LongProgress> Progress => Observable.Never<LongProgress>();
    public StoppableCommand<Unit, Result> Sync { get; }
}