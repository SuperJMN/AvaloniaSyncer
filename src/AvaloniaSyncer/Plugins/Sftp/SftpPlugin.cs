using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Plugins.Sftp.Configuration;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Sftp;

namespace AvaloniaSyncer.Plugins.Sftp;

public class SftpPlugin : ReactiveValidationObject, IFileSystemPlugin
{
    private readonly Maybe<ILogger> logger;

    public SftpPlugin(Maybe<ILogger> logger)
    {
        this.logger = logger;
        this.ValidationRule(x => x.Path, s => !string.IsNullOrEmpty(s), "Path cannot be empty");
        this.ValidationRule(x => x.Username, s => !string.IsNullOrEmpty(s), "Username cannot be empty");
        this.ValidationRule(x => x.Password, s => !string.IsNullOrEmpty(s), "Password cannot be empty");
        this.ValidationRule(x => x.Port, s => s > 0 && s < ushort.MaxValue, "Invalid port");
        this.ValidationRule(x => x.Host, s => !string.IsNullOrEmpty(s), "Host cannot be empty");
        Config = new ConfigViewModel();
        this.WhenAnyValue(x => x.Config.SelectedProfile)
            .WhereNotNull()
            .Do(model =>
            {
                Username = model.Username;
                Host = model.Host;
                Password = model.Password;
                Port= model.Port;
            })
            .Subscribe();

        Config.Load.Execute().Subscribe();
    }

    public ConfigViewModel Config { get; set; }

    [Reactive] public int Port { get; set; } = 22;

    [Reactive] public string Username { get; set; } = "";

    [Reactive] public string Host { get; set; } = "";

    [Reactive] public string Password { get; set; } = "";

    public async Task<Result<IFileSystem>> FileSystem()
    {
        var f = await SftpFileSystem.Create(Host, Port, Username, Password, logger);
        return f.Map(system => (IFileSystem) system);
    }

    [Reactive] public string Path { get; set; } = "";

    public IObservable<bool> IsValid => this.IsValid();
}
