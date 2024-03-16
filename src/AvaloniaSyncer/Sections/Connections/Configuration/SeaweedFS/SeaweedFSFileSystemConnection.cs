using System;
using System.Net.Http;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
#if DEBUG
using HttpClient.Extensions.LoggingHttpMessageHandler;
#endif
using Serilog;
using Zafiro.FileSystem;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;

internal class SeaweedFSFileSystemConnection : IZafiroFileSystemConnection
{
    public Uri Uri { get; }

    private readonly Maybe<ILogger> logger;

    public SeaweedFSFileSystemConnection(Guid id, string name, Uri uri, Maybe<ILogger> logger)
    {
        Id = id;
        Name = name;
        Uri = uri;
        this.logger = logger;
    }

    public Guid Id { get; set; }

    public async Task<Result<IDisposableFilesystemRoot>> FileSystem()
    {
        var handler = GetHandler();

        var httpClient = new System.Net.Http.HttpClient(handler)
        {
            BaseAddress = Uri,
            Timeout = TimeSpan.FromDays(1),
        };

        var seaweedFSClient = new SeaweedFSClient(httpClient);
        IFileSystemRoot seaweedFileSystem = new FileSystemRoot(new ObservableFileSystem(new SeaweedFileSystem(seaweedFSClient, logger)));
        return Result.Success((IDisposableFilesystemRoot)new DisposableFileSystemRoot(seaweedFileSystem, () => { }));
    }

    private HttpMessageHandler GetHandler()
    {
        return logger.Match<HttpMessageHandler, ILogger>(l =>
        {
#if DEBUG
            if (OperatingSystem.IsWindows())
            {
                LoggingHttpMessageHandler handler = new LoggingHttpMessageHandler(new LoggerAdapter(l));   
                handler.InnerHandler = new HttpClientHandler();
                return handler;
            }
            else
            {
                return new HttpClientHandler();
            }
#else
            return new HttpClientHandler();
#endif
        }, () => new HttpClientHandler());
    }

    public string Name { get; }
}