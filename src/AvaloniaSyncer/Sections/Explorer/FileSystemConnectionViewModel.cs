using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AvaloniaSyncer.Sections.Connections.Configuration.Sftp;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Explorer;

public class FileSystemConnectionViewModel : ReactiveObject, IZafiroFileSystemConnectionViewModel, IDisposable
{
    private readonly IZafiroFileSystemConnection connection;
    private readonly ObservableAsPropertyHelper<IFileSystemExplorer?> explorer;
    private readonly CompositeDisposable disposable = new();

    public FileSystemConnectionViewModel(IZafiroFileSystemConnection connection, INotificationService notificationService, IClipboard clipboardViewModel, ITransferManager transferManager, IContentOpener contentOpener)
    {
        this.connection = connection;

        var canLoad = new Subject<bool>();
        Load = ReactiveCommand.CreateFromObservable(() => Observable.FromAsync(connection.FileSystem), canLoad).DisposeWith(disposable);
        Refresh = ReactiveCommand.CreateFromObservable(() => Observable.FromAsync(connection.FileSystem)).DisposeWith(disposable);

        var loadsAndRefreshes = Load.Merge(Refresh);
        var fileSystems = loadsAndRefreshes.Successes();

        loadsAndRefreshes.HandleErrorsWith(notificationService).DisposeWith(disposable);

        // Disposes old filesystem when new one is created
        new SerialDisposer<IDisposableFilesystemRoot>(fileSystems).DisposeWith(disposable);

        explorer = fileSystems
            .Select(system => new FileSystemExplorer(system, notificationService, clipboardViewModel, transferManager, contentOpener))
            .ToProperty(this, x => x.FileSystemExplorer)
            .DisposeWith(disposable);

        this.WhenAnyValue(x => x.FileSystemExplorer, selector: s => s is null).Subscribe(canLoad).DisposeWith(disposable);
    }

    public ReactiveCommand<Unit, Result<IDisposableFilesystemRoot>> Refresh { get; set; }

    public IFileSystemExplorer? FileSystemExplorer => explorer.Value;

    public ReactiveCommand<Unit, Result<IDisposableFilesystemRoot>> Load { get; set; }

    public string Name => connection.Name;

    public void Dispose()
    {
        disposable.Dispose();
    }
}