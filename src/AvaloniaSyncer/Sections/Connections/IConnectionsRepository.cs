using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;

namespace AvaloniaSyncer.Sections.Connections;

public interface IConnectionsRepository
{
    ReadOnlyObservableCollection<IZafiroFileSystemConnection> Connections { get; }
    Task AddOrUpdate(IZafiroFileSystemConnection connection);
}