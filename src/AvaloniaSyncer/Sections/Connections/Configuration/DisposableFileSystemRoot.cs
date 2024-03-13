using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.Connections.Configuration;

public class DisposableFileSystemRoot : IDisposableFilesystemRoot
{
    private readonly IFileSystemRoot disposableFilesystemRootImplementation;
    private readonly Action onDispose;

    public DisposableFileSystemRoot(IFileSystemRoot fileSystemRoot, Action onDispose)
    {
        disposableFilesystemRootImplementation = fileSystemRoot;
        this.onDispose = onDispose;
    }

    public void Dispose()
    {
        onDispose();
    }

    public Task<Result> CreateFile(ZafiroPath path) => disposableFilesystemRootImplementation.CreateFile(path);

    public IObservable<byte> GetFileContents(ZafiroPath path) => disposableFilesystemRootImplementation.GetFileContents(path);

    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes, CancellationToken cancellationToken = default) => disposableFilesystemRootImplementation.SetFileContents(path, bytes, cancellationToken);

    public Task<Result> CreateDirectory(ZafiroPath path) => disposableFilesystemRootImplementation.CreateDirectory(path);

    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path) => disposableFilesystemRootImplementation.GetFileProperties(path);

    public Task<Result<IDictionary<HashMethod, byte[]>>> GetHashes(ZafiroPath path) => disposableFilesystemRootImplementation.GetHashes(path);

    public Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path) => disposableFilesystemRootImplementation.GetDirectoryProperties(path);

    public Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path, CancellationToken ct = new()) => disposableFilesystemRootImplementation.GetFilePaths(path, ct);

    public Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path, CancellationToken ct = new()) => disposableFilesystemRootImplementation.GetDirectoryPaths(path, ct);

    public Task<Result<bool>> ExistDirectory(ZafiroPath path) => disposableFilesystemRootImplementation.ExistDirectory(path);

    public Task<Result<bool>> ExistFile(ZafiroPath path) => disposableFilesystemRootImplementation.ExistFile(path);

    public Task<Result> DeleteFile(ZafiroPath path) => disposableFilesystemRootImplementation.DeleteFile(path);

    public Task<Result> DeleteDirectory(ZafiroPath path) => disposableFilesystemRootImplementation.DeleteDirectory(path);

    public IObservable<FileSystemChange> Changed => disposableFilesystemRootImplementation.Changed;

    public IZafiroFile GetFile(ZafiroPath path) => disposableFilesystemRootImplementation.GetFile(path);

    public IZafiroDirectory GetDirectory(ZafiroPath path) => disposableFilesystemRootImplementation.GetDirectory(path);

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles(ZafiroPath path, CancellationToken ct = new()) => disposableFilesystemRootImplementation.GetFiles(path, ct);

    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories(ZafiroPath path, CancellationToken ct = new()) => disposableFilesystemRootImplementation.GetDirectories(path, ct);

    public Task<Result<Stream>> GetFileData(ZafiroPath path) => disposableFilesystemRootImplementation.GetFileData(path);

    public Task<Result> SetFileData(ZafiroPath path, Stream stream, CancellationToken ct = default) => disposableFilesystemRootImplementation.SetFileData(path, stream, ct);
}