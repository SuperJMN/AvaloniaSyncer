﻿using System;
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
using Zafiro.Mixins;

namespace AvaloniaSyncer.Sections.Synchronization.Actions;

public class CopyAction : ReactiveObject, IFileActionViewModel
{
    private readonly CopyFileAction copyAction;
    private readonly BehaviorSubject<bool> isSyncing = new(false);

    private CopyAction(CopyFileAction copyAction)
    {
        this.copyAction = copyAction;
        Progress = copyAction.Progress;
        Description = $"Copy {copyAction.Source} over {copyAction.Destination}";
    }

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

    public static Task<Result<CopyAction>> Create(IZafiroFile source, IZafiroFile destination)
    {
        return CopyFileAction.Create(source, destination).Map(action => new CopyAction(action));
    }
}