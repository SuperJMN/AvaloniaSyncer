using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;

namespace AvaloniaSyncer.Sections.Connections;

public interface IConnectionsRepository
{
    ReadOnlyObservableCollection<IFileSystemConnection> Connections { get; }
    Task AddOrUpdate(IFileSystemConnection connection);
}