using System.Reactive;
using AvaloniaSyncer.Sections.Connections.Configuration.Sftp;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.Explorer;

public interface IZafiroFileSystemConnectionViewModel
{
    ReactiveCommand<Unit, Result<IDisposableFilesystemRoot>> Load { get; set; }
    string Name { get; }
    ReactiveCommand<Unit, Result<IDisposableFilesystemRoot>> Refresh { get; set; }
}