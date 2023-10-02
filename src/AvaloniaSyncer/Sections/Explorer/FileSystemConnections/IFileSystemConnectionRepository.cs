using System.Collections.ObjectModel;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections;

public interface IFileSystemConnectionRepository
{
    ReadOnlyObservableCollection<Serialization.IFileSystemConnection> Connections { get; }
}