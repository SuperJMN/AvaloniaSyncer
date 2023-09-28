using System.Reactive;
using System.Reactive.Linq;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.ViewModels;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Explorer;

public class FileSystemConnectionViewModel : ReactiveObject
{
    private readonly IFileSystemConnection connection;
    private readonly ObservableAsPropertyHelper<FolderContentsViewModel> explorer;

    public FileSystemConnectionViewModel(IFileSystemConnection connection, INotificationService notificationService)
    {
        this.connection = connection;

        Load = ReactiveCommand.CreateFromObservable(() => Observable.FromAsync(connection.FileSystem));
        explorer = Load
            .Successes()
            .Select(system => new FolderContentsViewModel(system, DirectoryListing.GetAll, notificationService))
            .ToProperty(this, x => x.Explorer);
    }

    public FolderContentsViewModel Explorer => explorer.Value;

    public ReactiveCommand<Unit, Result<IFileSystem>> Load { get; set; }

    public string Name => connection.Name;
}