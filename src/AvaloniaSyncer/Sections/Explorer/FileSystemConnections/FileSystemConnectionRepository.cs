using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections;

public class FileSystemConnectionRepository : IZafiroFileSystemConnectionRepository
{
    private readonly SourceCache<Serialization.IZafiroFileSystemConnection, string> systems = new(x => x.Name);

    public FileSystemConnectionRepository(IEnumerable<Serialization.IZafiroFileSystemConnection> configurations)
    {
        systems.AddOrUpdate(configurations);
        var changes = systems.Connect();
        changes.Bind(out var collection).Subscribe();
        Connections = collection;
    }

    public ReadOnlyObservableCollection<Serialization.IZafiroFileSystemConnection> Connections { get; }
}