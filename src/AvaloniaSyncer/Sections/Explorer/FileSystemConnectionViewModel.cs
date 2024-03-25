using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AvaloniaSyncer.Sections.Connections.Configuration;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Explorer;

public class FileSystemConnectionViewModel : ReactiveObject, IZafiroFileSystemConnectionViewModel, IDisposable
{
    private readonly CompositeDisposable disposable = new();
    private readonly ObservableAsPropertyHelper<IFileSystemExplorer?> explorer;

    public FileSystemConnectionViewModel(IZafiroFileSystemConnection connection, INotificationService notificationService, IClipboard clipboardViewModel, ITransferManager transferManager, IContentOpener contentOpener)
    {
        Connection = connection;

        var canLoad = new Subject<bool>();
        Load = ReactiveCommand.CreateFromObservable(() => Observable.FromAsync(connection.FileSystem), canLoad).DisposeWith(disposable);
        Refresh = ReactiveCommand.CreateFromObservable(() => Observable.FromAsync(connection.FileSystem)).DisposeWith(disposable);

        var loadsAndRefreshes = Load.Merge(Refresh);
        var fileSystems = loadsAndRefreshes.Successes();

        loadsAndRefreshes.HandleErrorsWith(notificationService).DisposeWith(disposable);

        // Disposes old filesystem when new one is created
        new SerialDisposer<IDisposableFilesystemRoot>(fileSystems).DisposeWith(disposable);

        explorer = fileSystems
            .Select(system => new FileSystemExplorer(new ExplorerContext( notificationService, clipboardViewModel, transferManager, contentOpener), system))
            .ToProperty(this, x => x.FileSystemExplorer)
            .DisposeWith(disposable);

        this.WhenAnyValue(x => x.FileSystemExplorer, selector: s => s is null).Subscribe(canLoad).DisposeWith(disposable);
    }

    public IZafiroFileSystemConnection Connection { get; }

    public IFileSystemExplorer? FileSystemExplorer => explorer.Value;

    public void Dispose()
    {
        disposable.Dispose();
    }

    public ReactiveCommand<Unit, Result<IDisposableFilesystemRoot>> Refresh { get; set; }

    public ReactiveCommand<Unit, Result<IDisposableFilesystemRoot>> Load { get; set; }

    public string Name => Connection.Name;
}