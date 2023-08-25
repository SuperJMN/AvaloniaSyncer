using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Plugins.SeaweedFS_new.Configuration;
using CSharpFunctionalExtensions;
using HttpClient.Extensions.LoggingHttpMessageHandler;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace AvaloniaSyncer.Plugins.SeaweedFS_new;

public class SessionViewModel : ReactiveValidationObject, ISession
{
    private readonly Maybe<ILogger> logger;

    public SessionViewModel(Maybe<ILogger> logger)
    {
        this.logger = logger;
        this.ValidationRule(x => x.Path, s => !string.IsNullOrEmpty(s), "Invalid path");
        var configurationViewModel = new ConfigurationViewModel();
        Configuration = configurationViewModel;

        Directory = Configuration.IsValid()
            .Where(b => b)
            .Select(_ => this.WhenAnyValue(x => x.Path, s => s.Configuration)
                .SelectMany(tuple => GetDirectory(tuple.Item1, tuple.Item2)).Successes())
            .Switch();
    }

    private ConfigurationViewModel Configuration { get; }

    [Reactive] public string Path { get; set; } = "";

    public IObservable<IZafiroDirectory> Directory { get; }
    public IObservable<bool> IsValid => this.IsValid();

    private Task<Result<IZafiroDirectory>> GetDirectory(string path, ConfigurationViewModel configuration)
    {
        return GetFileSystem(configuration.Address).Bind(system => system.GetDirectory(path));
    }

    private Task<Result<IFileSystem>> GetFileSystem(string address)
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
                BaseAddress = new Uri(address),
                Timeout = TimeSpan.FromDays(1)
            };
            var seaweedFSClient = new SeaweedFSClient(httpClient);
            return (IFileSystem) new SeaweedFileSystem(seaweedFSClient, logger);
        });

        return Task.FromResult(fileSystem);
    }
}