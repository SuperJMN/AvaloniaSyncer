using System;
using System.Net.Http;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using HttpClient.Extensions.LoggingHttpMessageHandler;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections;

internal class SeaweedFileSystemConnection : Serialization.IFileSystemConnection
{
    public Uri Uri { get; }

    private readonly Maybe<ILogger> logger;

    public SeaweedFileSystemConnection(string name, Uri uri, Maybe<ILogger> logger)
    {
        Uri = uri;
        this.logger = logger;
        Name = name;
    }

    public Task<Result<IFileSystem>> FileSystem()
    {
        var handler = GetHandler();

        var httpClient = new System.Net.Http.HttpClient(handler)
        {
            BaseAddress = Uri, 
            Timeout = TimeSpan.FromDays(1),
        };

        var seaweedFSClient = new SeaweedFSClient(httpClient);
        IFileSystem seaweedFileSystem = new SeaweedFileSystem(seaweedFSClient, logger);
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

    public string Name { get; set; }
}