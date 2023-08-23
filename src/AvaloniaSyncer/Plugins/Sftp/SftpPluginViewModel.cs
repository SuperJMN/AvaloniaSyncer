using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Plugins.Sftp.Configuration;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Sftp;

namespace AvaloniaSyncer.Plugins.Sftp;

public class SftpPluginViewModel : ReactiveValidationObject, ISession
{
    private readonly Maybe<ILogger> logger;
    private readonly ObservableAsPropertyHelper<List<ProfileDto>> profiles;

    public SftpPluginViewModel(Maybe<ILogger> logger)
    {
        this.logger = logger;
        this.ValidationRule(x => x.Path, s => !string.IsNullOrEmpty(s), "Path cannot be empty");
        this.ValidationRule(x => x.Username, s => !string.IsNullOrEmpty(s), "Username cannot be empty");
        this.ValidationRule(x => x.Password, s => !string.IsNullOrEmpty(s), "Password cannot be empty");
        this.ValidationRule(x => x.Port, s => s > 0 && s < ushort.MaxValue, "Invalid port");
        this.ValidationRule(x => x.Host, s => !string.IsNullOrEmpty(s), "Host cannot be empty");
        this.WhenAnyValue(x => x.SelectedProfile)
            .WhereNotNull()
            .Do(model =>
            {
                Username = model.Username;
                Host = model.Host;
                Password = model.Password;
                Port = model.Port;
            })
            .Subscribe();
        profiles = Observable.FromAsync(() => new Repository().Load()).Successes().Select(x => x.Profiles).ToProperty(this, x => x.Profiles);
    }

    [Reactive] public ProfileDto? SelectedProfile { get; set; }

    public List<ProfileDto> Profiles => profiles.Value;

    [Reactive] public int Port { get; set; } = 22;

    [Reactive] public string Username { get; set; } = "";

    [Reactive] public string Host { get; set; } = "";

    [Reactive] public string Password { get; set; } = "";

    [Reactive] public string Path { get; set; } = "";

    public async Task<Result<IFileSystem>> FileSystem()
    {
        var f = await SftpFileSystem.Create(Host, Port, Username, Password, logger);
        return f.Map(system => (IFileSystem) system);
    }

    public IObservable<bool> IsValid => this.IsValid();
}