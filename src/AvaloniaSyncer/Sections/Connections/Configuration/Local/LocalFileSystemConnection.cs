using System;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Local;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Local;

public class LocalFileSystemConnection : IZafiroFileSystemConnection
{
    public LocalFileSystemConnection(Guid id, string name)
    {
        Name = name;
        Id = id;
    }

    public Guid Id { get; set; }

    public Task<Result<IDisposableFilesystemRoot>> FileSystem()
    {
        return Task.FromResult(Result.Success<IDisposableFilesystemRoot>(new DisposableFileSystemRoot(new FileSystemRoot(new ObservableFileSystem(LocalFileSystem.Create())), () => { })));
    }

    public string Name { get; }
}