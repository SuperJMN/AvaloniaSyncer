using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Local;

namespace AvaloniaSyncer;

public class LocalPlugin : IFileSystemPlugin
{
    public Result<IFileSystem> FileSystem() => new LocalFileSystem();
    public string Path { get; set; }
}