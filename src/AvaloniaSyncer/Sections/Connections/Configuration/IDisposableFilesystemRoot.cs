using System;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.Connections.Configuration;

public interface IDisposableFilesystemRoot : IDisposable, IFileSystemRoot;