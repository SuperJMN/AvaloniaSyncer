using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace AvaloniaSyncer;

public interface IFileSystemPlugin
{
    public Task<Result<IFileSystem>> FileSystem();
    public string Path { get; set; }
}