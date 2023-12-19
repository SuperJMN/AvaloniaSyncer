using System;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using Renci.SshNet;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Sftp;

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

    public async Task<Result<IFileSystemRoot>> FileSystem()
    {
        return new FileSystemRoot(new ObservableFileSystem(new SftpFileSystem(new SftpClient(Parameters.Host, Parameters.Username, Parameters.Username))));
    }

    public string Name { get; }
}