using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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

public class FileSystemConnectionViewModel : ReactiveObject, IZafiroFileSystemConnectionViewModel
{
    private readonly IZafiroFileSystemConnection connection;
    private readonly ObservableAsPropertyHelper<IFileSystemExplorer?> explorer;

    public FileSystemConnectionViewModel(IZafiroFileSystemConnection connection, INotificationService notificationService, IClipboard clipboardViewModel, ITransferManager transferManager)
    {
        this.connection = connection;

        var canLoad = new Subject<bool>();
        Load = ReactiveCommand.CreateFromObservable(() => Observable.FromAsync(connection.FileSystem), canLoad);
        Refresh = ReactiveCommand.CreateFromObservable(() => Observable.FromAsync(connection.FileSystem));

        explorer = Load.Merge(Refresh)
            .Successes()
            .Select(system => new FileSystemExplorer(system, notificationService, clipboardViewModel, transferManager, null))
            .ToProperty(this, x => x.FileSystemExplorer);

        this.WhenAnyValue(x => x.FileSystemExplorer, selector: s => s is null).Subscribe(canLoad);
    }

    public ReactiveCommand<Unit, Result<IFileSystemRoot>> Refresh { get; set; }

    public IFileSystemExplorer? FileSystemExplorer => explorer.Value;

    public ReactiveCommand<Unit, Result<IFileSystemRoot>> Load { get; set; }

    public string Name => connection.Name;
}