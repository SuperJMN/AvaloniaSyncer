using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections;

public class FileSystemConnectionRepository : IFileSystemConnectionRepository
{
    private readonly SourceCache<Serialization.IFileSystemConnection, string> systems = new(x => x.Name);

    public FileSystemConnectionRepository(IEnumerable<Serialization.IFileSystemConnection> configurations)
    {
        systems.AddOrUpdate(configurations);
        var changes = systems.Connect();
        changes.Bind(out var collection).Subscribe();
        Connections = collection;
    }

    public ReadOnlyObservableCollection<Serialization.IFileSystemConnection> Connections { get; }
}