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

public class ExplorerSectionViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<FileSystemConnectionViewModel> connections;

    public ExplorerSectionViewModel(IConnectionsRepository repository, INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager, Maybe<ILogger> logger)
    {
        repository.Connections
            .ToObservableChangeSet(x => x.Name)
            .Transform(connection => new FileSystemConnectionViewModel(connection, notificationService, clipboard, transferManager))
            .Bind(out connections)
            .Subscribe();
    }
    
    public ReadOnlyObservableCollection<FileSystemConnectionViewModel> Connections => connections;
}