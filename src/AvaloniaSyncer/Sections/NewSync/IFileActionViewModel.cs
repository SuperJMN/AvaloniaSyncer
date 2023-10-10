using System;
using System.ComponentModel;
using System.Reactive;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.NewSync;

public interface IFileActionViewModel : INotifyPropertyChanged
{
    public string Description { get; }
    public IObservable<LongProgress> Progress { get; }
    public StoppableCommand<Unit, Result> Sync { get; }
    public IObservable<bool> IsSyncing { get; }
    bool IsIgnored { get; }
    bool IsSynced { get; }
}