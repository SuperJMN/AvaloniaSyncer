using System.Threading.Tasks;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using Zafiro;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Local;

namespace AvaloniaSyncer;

public class LocalPlugin : ViewModelBase, IFileSystemPlugin
{
    public Task<Result<IFileSystem>> FileSystem() => Task.FromResult(Result.Success<IFileSystem>(new LocalFileSystem()));
    public string Path { get; set; }
}