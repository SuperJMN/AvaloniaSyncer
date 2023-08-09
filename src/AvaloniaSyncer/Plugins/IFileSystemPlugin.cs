using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Plugins;

public interface IFileSystemPlugin
{
    public Task<Result<IFileSystem>> FileSystem();
    public string Path { get; set; }
    IObservable<bool> IsValid { get; }
}