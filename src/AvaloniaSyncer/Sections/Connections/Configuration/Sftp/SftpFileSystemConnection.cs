using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Sftp;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Sftp;

internal class SftpFileSystemConnection : IFileSystemConnection
{
    public SftpConnectionParameters Parameters { get; }

    public SftpFileSystemConnection(string name, SftpConnectionParameters parameters)
    {
        Parameters = parameters;
        Name = name;
    }

    public async Task<Result<IFileSystem>> FileSystem()
    {
        var result = await SftpFileSystem.Create(Parameters.Host, Parameters.Port, Parameters.Username, Parameters.Password, Maybe<ILogger>.None);
        return result.Map(system => (IFileSystem)system);
    }

    public string Name { get; }
}