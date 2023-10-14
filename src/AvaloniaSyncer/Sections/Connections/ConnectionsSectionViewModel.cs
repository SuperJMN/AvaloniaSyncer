using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using DynamicData;
using ReactiveUI;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Connections;

public class ConnectionsSectionViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<IConfiguration> connections;

    public ConnectionsSectionViewModel(IConnectionsRepository repo, INotificationService notificationService)
    {
        repo.Connections.AsObservableChangeSet().Transform(Mapper.ToEditable).Bind(out connections).Subscribe();
    }
    
    public ReadOnlyObservableCollection<IConfiguration> Connections => connections;
}

public interface IConnectionsRepository
{
    ReadOnlyObservableCollection<IFileSystemConnection> Connections { get; }
}

class ConnectionsRepository : IConnectionsRepository
{
    private readonly SourceCache<IFileSystemConnection, string> connectionsSource = new(x => x.Name);
    private readonly ReadOnlyObservableCollection<IFileSystemConnection> connections;

    public ConnectionsRepository(IEnumerable<IFileSystemConnection> connections)
    {
        connectionsSource.AddOrUpdate(connections);
        connectionsSource.Connect().Bind(out this.connections).Subscribe();
    }

    public ReadOnlyObservableCollection<IFileSystemConnection> Connections => connections;
}