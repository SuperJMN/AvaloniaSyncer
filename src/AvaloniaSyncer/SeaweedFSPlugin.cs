using System;
using System.Net.Http;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using HttpClient.Extensions.LoggingHttpMessageHandler;
using Zafiro.FileSystem;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using ILogger = Serilog.ILogger;

namespace AvaloniaSyncer;

public class SeaweedFSPlugin : IFileSystemPlugin
{
    private readonly Maybe<ILogger> logger;

    public SeaweedFSPlugin(Maybe<ILogger> logger)
    {
        this.logger = logger;
    }

    public Task<Result<IFileSystem>> FileSystem()
    {
        var fileSystem = Result.Try(() =>
        {
            var handler = logger.Match<HttpMessageHandler, ILogger>(f =>
            {
                var loggingHttpMessageHandler = new LoggingHttpMessageHandler(new LoggerAdapter(f));
                loggingHttpMessageHandler.InnerHandler = new HttpClientHandler();
                return loggingHttpMessageHandler;
            }, () => new HttpClientHandler());
            var httpClient = new System.Net.Http.HttpClient(handler)
            {
                BaseAddress = new Uri(Address),
                Timeout = TimeSpan.FromDays(1)
            };
            var seaweedFSClient = new SeaweedFSClient(httpClient);
            return (IFileSystem) new SeaweedFileSystem(seaweedFSClient);
        });

        return Task.FromResult(fileSystem);
    }

    public string Path { get; set; }
    public string Address { get; set; }
    
}