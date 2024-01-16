using System;
using System.Collections.Generic;
using System.Reactive;
using AvaloniaSyncer.Sections.Synchronization.Actions;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Synchronization;

public interface ISyncSessionViewModel
{
    IObservable<LongProgress> Progress { get; }
    IStoppableCommand<Unit, Result> Sync { get; }
    IEnumerable<IFileActionViewModel> SyncActions { get; }
    IStoppableCommand<Unit, Result<IEnumerable<IFileActionViewModel>>> Analyze { get; }
    IObservable<bool> IsSyncing { get; }
    IObservable<bool> IsAnalyzing { get; }
    string Description { get; }
}