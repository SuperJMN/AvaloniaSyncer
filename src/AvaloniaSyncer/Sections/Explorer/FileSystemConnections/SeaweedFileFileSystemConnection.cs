using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections;

internal class SeaweedFileFileSystemConnection : Serialization.IFileSystemConnection
{
    private readonly Uri uri;

    public SeaweedFileFileSystemConnection(string name, Uri uri)
    {
        this.uri = uri;
        Name = name;
    }

    public Task<Result<IFileSystem>> FileSystem()
    {
        var seaweedFSClient = new SeaweedFSClient(new System.Net.Http.HttpClient() { BaseAddress = uri });
        IFileSystem seaweedFileSystem = new SeaweedFileSystem(seaweedFSClient, Maybe<ILogger>.None);
        return Task.FromResult(Result.Success(seaweedFileSystem));
    }

    public string Name { get; set; }
}