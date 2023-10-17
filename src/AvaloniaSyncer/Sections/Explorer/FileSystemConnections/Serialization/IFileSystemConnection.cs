using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;

public interface IFileSystemConnection
{
    Task<Result<IFileSystem>> FileSystem();
    string Name { get; }
}