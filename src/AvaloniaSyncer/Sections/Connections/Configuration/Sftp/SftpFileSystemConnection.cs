using System;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Sftp;

internal class SftpFileSystemConnection : IZafiroFileSystemConnection
{
    public SftpFileSystemConnection(Guid id, string name, SftpConnectionParameters parameters)
    {
        Parameters = parameters;
        Id = id;
        Name = name;
    }

    public SftpConnectionParameters Parameters { get; }

    public Guid Id { get; set; }

    public Task<Result<IDisposableFilesystemRoot>> FileSystem()
    {
        return SftpFileSystemRoot.Create(Parameters.Host, Parameters.Username, Parameters.Password);
    }

    public string Name { get; }
}