using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;

public interface IFileSystemConnection
{
    public Guid Id { get; set; }
    Task<Result<IFileSystem>> FileSystem();
    string Name { get; }
}