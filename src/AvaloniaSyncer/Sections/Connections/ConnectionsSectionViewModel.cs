using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Connections;

public class ConnectionsSectionViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<ReadOnlyObservableCollection<IFileSystemConnection>> connections;

    public ConnectionsSectionViewModel(Func<Task<Result<IConnectionsRepository>>> repo, INotificationService notificationService)
    {
        LoadConnections = ReactiveCommand.CreateFromTask(() => repo().Map(r => r.Connections));
        connections = LoadConnections.Successes().ToProperty(this, model => model.Connections);
        LoadConnections.HandleErrorsWith(notificationService);
    }

    public ReactiveCommand<Unit, Result<ReadOnlyObservableCollection<IFileSystemConnection>>> LoadConnections { get; }

    public ReadOnlyObservableCollection<IFileSystemConnection> Connections => connections.Value;
}

public interface IConnectionsRepository
{
    ReadOnlyObservableCollection<IFileSystemConnection> Connections { get; }
}

class ConnectionsRepository : IConnectionsRepository
{
    private readonly SourceCache<IFileSystemConnection, string> connectionsSource = new(x => x.Name);
    private readonly ReadOnlyObservableCollection<IFileSystemConnection> connections;

    private ConnectionsRepository(IEnumerable<IFileSystemConnection> connections)
    {
        connectionsSource.AddOrUpdate(connections);
        connectionsSource.Connect().Bind(out this.connections).Subscribe();
    }

    public ReadOnlyObservableCollection<IFileSystemConnection> Connections => connections;

    public static async Task<Result<ConnectionsRepository>> Create(Maybe<ILogger> logger)
    {
        var store = new ConfigurationStore(() => File.OpenRead("Connections.json"), () => File.OpenWrite("Connections.json"));
        var loadResult = await store.Load();
        return loadResult.Map(enumerable => new ConnectionsRepository(enumerable.Select(x => Mapper.ToSystem(x, logger))));
    }
}