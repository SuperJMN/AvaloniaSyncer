using System.Threading.Tasks;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using ReactiveUI.Fody.Helpers;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Sftp;

namespace AvaloniaSyncer;

public class SftpPlugin : ViewModelBase, IFileSystemPlugin
{
    public async Task<Result<IFileSystem>> FileSystem()
    {
        var f = await SftpFileSystem.Create(Host, Port, Username, Password);
        return f.Map(system => (IFileSystem) system);
    }

    public int Port { get; set; }

    [Reactive]
    public string Username { get; set; }

    [Reactive]
    public string Host { get; set; }

    [Reactive]
    public string Password { get; set; }

    [Reactive]
    public string Path { get; set; }
}