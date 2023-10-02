using System.Reactive;
using System.Reactive.Linq;
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

public class FileSystemConnectionViewModel : ReactiveObject
{
    private readonly IFileSystemConnection connection;
    private readonly ObservableAsPropertyHelper<IFileSystemExplorer> explorer;

    public FileSystemConnectionViewModel(IFileSystemConnection connection, INotificationService notificationService, IClipboard clipboardViewModel, ITransferManager transferManager)
    {
        this.connection = connection;

        Load = ReactiveCommand.CreateFromObservable(() => Observable.FromAsync(connection.FileSystem));
        explorer = Load
            .Successes()
            .Select(system => new FileSystemExplorer(system, DirectoryListing.GetAll, notificationService, clipboardViewModel, transferManager))
            .ToProperty(this, x => x.FileSystemExplorer);
    }

    public IFileSystemExplorer FileSystemExplorer => explorer.Value;

    public ReactiveCommand<Unit, Result<IFileSystem>> Load { get; set; }

    public string Name => connection.Name;
}