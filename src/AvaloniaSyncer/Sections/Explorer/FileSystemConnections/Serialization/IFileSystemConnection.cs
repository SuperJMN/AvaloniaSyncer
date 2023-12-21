using System;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Connections.Configuration.Sftp;
using CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;

public interface IZafiroFileSystemConnection
{
    public Guid Id { get; }
    Task<Result<IDisposableFilesystemRoot>> FileSystem();
    string Name { get; }
}