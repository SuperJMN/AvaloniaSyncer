using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using DynamicData;
using Serilog;

namespace AvaloniaSyncer.Sections.Connections;

internal class ConnectionsRepository : IConnectionsRepository
{
    private readonly Maybe<ILogger> logger;
    private readonly ReadOnlyObservableCollection<IZafiroFileSystemConnection> connections;
    private readonly SourceCache<IZafiroFileSystemConnection, Guid> connectionsSource = new(x => x.Id);
    private readonly ConfigurationStore store;

    public ConnectionsRepository(IEnumerable<IZafiroFileSystemConnection> connections, Maybe<ILogger> logger, ConfigurationStore store)
    {
        this.store = store;
        this.logger = logger;
        connectionsSource.AddOrUpdate(connections);
        connectionsSource.Connect().Bind(out this.connections).Subscribe();
    }

    public ReadOnlyObservableCollection<IZafiroFileSystemConnection> Connections => connections;

    public async Task AddOrUpdate(IZafiroFileSystemConnection connection)
    {
        connectionsSource.AddOrUpdate(connection);
        var result = await store.Save(Connections.Select(Mapper.ToConfiguration));
        result.TapError(e => logger.Execute(l => l.Error($"Could not save the configuration file: {e}")));
    }

    public async Task Remove(Guid id)
    {
        connectionsSource.RemoveKey(id);
        var result = await store.Save(Connections.Select(Mapper.ToConfiguration));
        result.TapError(e => logger.Execute(l => l.Error($"Could not save the configuration file: {e}")));
    }
}