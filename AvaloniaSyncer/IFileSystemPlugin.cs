using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace AvaloniaSyncer;

public interface IFileSystemPlugin
{
    public Result<IFileSystem> FileSystem();
    public string Path { get; set; }
}