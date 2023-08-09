using System;
using System.Net.Http;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using HttpClient.Extensions.LoggingHttpMessageHandler;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace AvaloniaSyncer.Plugins.SeaweedFS;

public class SeaweedFSPlugin : ReactiveValidationObject, IFileSystemPlugin
{
    private readonly Maybe<ILogger> logger;

    public SeaweedFSPlugin(Maybe<ILogger> logger)
    {
        this.logger = logger;

        this.ValidationRule(x => x.Path, s => !string.IsNullOrEmpty(s), "Invalid path");
        this.ValidationRule(x => x.Address, s => !string.IsNullOrEmpty(s), "Invalid path");
    }

    [Reactive] public string Address { get; set; } = "";

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

    public IObservable<bool> IsValid => this.IsValid();

    [Reactive] public string Path { get; set; } = "";
}