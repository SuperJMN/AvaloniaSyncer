using System;
using System.Collections.ObjectModel;
using AvaloniaSyncer.Sections.Connections;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Serilog;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Explorer;

public interface IExplorerSectionViewModel
{
    ReadOnlyObservableCollection<IZafiroFileSystemConnectionViewModel> Connections { get; }
}

public class ExplorerSectionViewModel : ReactiveObject, IExplorerSectionViewModel
{
    private readonly ReadOnlyObservableCollection<IZafiroFileSystemConnectionViewModel> connections;

    public ExplorerSectionViewModel(IConnectionsRepository repository, INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager, Maybe<ILogger> logger)
    {
        repository.Connections
            .ToObservableChangeSet(x => x.Id)
            .Transform(connection => (IZafiroFileSystemConnectionViewModel)new FileSystemConnectionViewModel(connection, notificationService, clipboard, transferManager))
            .Sort(SortExpressionComparer<IZafiroFileSystemConnectionViewModel>.Ascending(x => x.Name))
            .Bind(out connections)
            .Subscribe();
    }
    
    public ReadOnlyObservableCollection<IZafiroFileSystemConnectionViewModel> Connections => connections;
}