using System.IO.Abstractions;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using Serilog;
using IFileSystem = Zafiro.FileSystem.IFileSystem;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Local;

public class LocalFileSystemConnection : IFileSystemConnection
{
    public LocalFileSystemConnection(string name)
    {
        Name = name;
    }

    public Task<Result<IFileSystem>> FileSystem()
    {
        return Task.FromResult(Result.Success<IFileSystem>(new Zafiro.FileSystem.Local.LocalFileSystem(new FileSystem(), Maybe<ILogger>.None)));
    }

    public string Name { get; }
}