using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.Explorer;

public interface IZafiroFileSystemConnectionViewModel
{
    ReactiveCommand<Unit, Result<IFileSystemRoot>> Load { get; set; }
    string Name { get; }
    ReactiveCommand<Unit, Result<IFileSystemRoot>> Refresh { get; set; }
}