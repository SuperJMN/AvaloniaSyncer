using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;

public class LocalFileSystemConnection : IFileSystemConnection
{
    public LocalFileSystemConnection(string name)
    {
        Name = name;
    }

    public Task<Result<IFileSystem>> FileSystem()
    {
        return Task.FromResult(Result.Success<IFileSystem>(new Zafiro.FileSystem.Local.LocalFileSystem(Maybe<ILogger>.None)));
    }

    public string Name { get; }
}