using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;
using IValidatable = Zafiro.UI.IValidatable;

namespace AvaloniaSyncer.Sections.Synchronization;

public class DirectorySelectionViewModel : ReactiveObject, IValidatable
{
    private readonly ObservableAsPropertyHelper<IZafiroDirectory> currentDirectoryWrapper;

    public DirectorySelectionViewModel(ReadOnlyCollection<IZafiroFileSystemConnection> connections, INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager)
    {
        Connections = connections;

        Explorer = this
            .WhenAnyValue(x => x.SelectedConnection)
            .WhereNotNull()
            .SelectMany(connection => connection.FileSystem().Map(fs => new FileSystemExplorer(fs, notificationService, clipboard, transferManager, null)))
            .Successes()
            .Publish()  // This is to force subscribers share a single subscription (they will share instances of the FileSystemExplorer, this way)
            .RefCount();

        var currentDirectory = Explorer.Select(x => x.CurrentDirectory).Switch();
        currentDirectoryWrapper = currentDirectory.Values().ToProperty(this, x => x.CurrentDirectory);
        IsValid = currentDirectory.Select(x => x.HasValue);
    }

    public IZafiroDirectory CurrentDirectory => currentDirectoryWrapper.Value;

    public ReadOnlyCollection<IZafiroFileSystemConnection> Connections { get; set; }
    [Reactive] public IZafiroFileSystemConnection? SelectedConnection { get; set; }
    public IObservable<FileSystemExplorer> Explorer { get; }
    public IObservable<bool> IsValid { get; }
}