using System;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Sftp;

public interface IDisposableFilesystemRoot : IDisposable, IFileSystemRoot
{
}