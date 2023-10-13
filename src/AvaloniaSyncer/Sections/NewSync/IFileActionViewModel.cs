using System;
using System.ComponentModel;
using ByteSizeLib;
using Zafiro.Actions;
using Zafiro.FileSystem.Actions;

namespace AvaloniaSyncer.Sections.NewSync;

public interface IFileActionViewModel : INotifyPropertyChanged, IFileAction
{
    public string Description { get; }
    bool IsIgnored { get; }
    bool IsSynced { get; }
    IObservable<LongProgress> Progress { get; }
    public IObservable<bool> IsSyncing { get; }
    public string? Error { get; }
    public IObservable<ByteSize> Rate { get; }
}