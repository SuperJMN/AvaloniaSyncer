using System;
using System.Net.Http;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using HttpClient.Extensions.LoggingHttpMessageHandler;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;

internal class SeaweedFileSystemConnection : IZafiroFileSystemConnection
{
    public Uri Uri { get; }

    private readonly Maybe<ILogger> logger;

    public SeaweedFileSystemConnection(Guid id, string name, Uri uri, Maybe<ILogger> logger)
    {
        Id = id;
        Name = name;
        Uri = uri;
        this.logger = logger;
    }

    public Guid Id { get; set; }

    public Task<Result<IFileSystemRoot>> FileSystem()
    {
        var handler = GetHandler();

        var httpClient = new System.Net.Http.HttpClient(handler)
        {
            BaseAddress = Uri,
            Timeout = TimeSpan.FromDays(1),
        };

        var seaweedFSClient = new SeaweedFSClient(httpClient);
        IFileSystemRoot seaweedFileSystem = new FileSystemRoot(new ObservableFileSystem(new SeaweedFileSystem(seaweedFSClient, logger)));
        return Task.FromResult(Result.Success(seaweedFileSystem));
    }

    private HttpMessageHandler GetHandler()
    {
        return logger.Match<HttpMessageHandler, ILogger>(f =>
        {
            var loggingHttpMessageHandler = new LoggingHttpMessageHandler(new LoggerAdapter(f));
            loggingHttpMessageHandler.InnerHandler = new HttpClientHandler();
            return loggingHttpMessageHandler;
        }, () => new HttpClientHandler());
    }

    public string Name { get; }
}