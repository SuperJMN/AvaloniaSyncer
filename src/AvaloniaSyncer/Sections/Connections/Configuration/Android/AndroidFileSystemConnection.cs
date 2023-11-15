using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.Android;
using IFileSystem = Zafiro.FileSystem.IFileSystem;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Android;

public class AndroidFileSystemConnection : IFileSystemConnection
{
    public AndroidFileSystemConnection(Guid id, string name)
    {
        Name = name;
        Id = id;
    }

    public Guid Id { get; set; }

    public Task<Result<IFileSystem>> FileSystem()
    {
        return Task.FromResult(Result.Success<IFileSystem>(new AndroidFileSystem(new FileSystem(), Maybe<ILogger>.None)));
    }

    public string Name { get; }
}