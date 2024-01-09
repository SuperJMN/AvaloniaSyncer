using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Renci.SshNet;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Sftp;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Sftp;

internal class SftpFileSystemRoot
{
    public static async Task<Result<IDisposableFilesystemRoot>> Create(string host, string username, string password)
    {
        return Result.Try(() =>
        {
            var sftpClient = new SftpClient(host, username, password);
            sftpClient.Connect();
            return sftpClient;
        }).Map(client => (IDisposableFilesystemRoot) new DisposableFileSystemRoot(new FileSystemRoot(new ObservableFileSystem(new SftpFileSystem(client))), client.Dispose));
    }
}