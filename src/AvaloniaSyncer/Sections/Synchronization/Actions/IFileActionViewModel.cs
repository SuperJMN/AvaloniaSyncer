using System;
using System.ComponentModel;
using ByteSizeLib;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Actions;

namespace AvaloniaSyncer.Sections.Synchronization.Actions;

public interface IFileActionViewModel : INotifyPropertyChanged, IFileAction
{
    public string Description { get; }
    bool IsIgnored { get; }
    bool IsSynced { get; }
    IObservable<LongProgress> Progress { get; }
    public IObservable<bool> IsSyncing { get; }
    public string? Error { get; }
    public IObservable<ByteSize> Rate { get; }
    string Comment { get; }
    Maybe<IZafiroFile> LeftFile { get; }
    Maybe<IZafiroFile> RightFile { get; }
}