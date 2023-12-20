using System.Collections.ObjectModel;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections;

public interface IZafiroFileSystemConnectionRepository
{
    ReadOnlyObservableCollection<Serialization.IZafiroFileSystemConnection> Connections { get; }
}