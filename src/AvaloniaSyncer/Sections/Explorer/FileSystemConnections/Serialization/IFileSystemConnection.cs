using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;

public interface IZafiroFileSystemConnection
{
    public Guid Id { get; set; }
    Task<Result<IFileSystemRoot>> FileSystem();
    string Name { get; }
}