using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Explorer;

public class ExplorerSectionViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<IList<FileSystemConnectionViewModel>> connections;

    public ExplorerSectionViewModel(INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager)
    {
        Load = ReactiveCommand.CreateFromObservable(() => Observable.FromAsync(() => GetConnections())
            .Successes()
            .SelectMany(x => x)
            .Select(connection => new FileSystemConnectionViewModel(connection, notificationService, clipboard, transferManager)).ToList());
        Load.Subscribe(model => { });

        connections = Load.ToProperty(this, x => x.Connections);
    }

    public ReactiveCommand<Unit, IList<FileSystemConnectionViewModel>> Load { get; }

    public IList<FileSystemConnectionViewModel> Connections => connections.Value;

    private async Task<Result<ReadOnlyObservableCollection<IFileSystemConnection>>> GetConnections()
    {
        var store = new ConfigurationStore(() => File.OpenRead("Connections.json"), () => File.OpenWrite("Connections.json"));
        var configs = await Async.Await(() => store.Load())
            .Map(enumerable => enumerable.Select(connection => Mapper.ToSystem(connection)))
            .Map(enumerable => new FileSystemConnectionRepository(enumerable))
            .Map(r => r.Connections);

        return configs;
    }
}