using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.Explorer;

public interface IFileSystemConnectionViewModel
{
    ReactiveCommand<Unit, Result<IFileSystem>> Load { get; set; }
    string Name { get; }
    ReactiveCommand<Unit, Result<IFileSystem>> Refresh { get; set; }
}