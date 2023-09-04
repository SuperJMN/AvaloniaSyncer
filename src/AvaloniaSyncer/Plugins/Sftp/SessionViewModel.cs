using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaSyncer.Plugins.Sftp.Configuration;
using AvaloniaSyncer.Plugins.Sftp.Settings;
using AvaloniaSyncer.Settings;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Sftp;
using Zafiro.UI;

namespace AvaloniaSyncer.Plugins.Sftp;

public class SessionViewModel : ReactiveValidationObject, ISession
{
    private readonly Maybe<ILogger> logger;

    public SessionViewModel(Func<IFileSystem, IFolderPicker> folderPicker, Maybe<ILogger> logger)
    {
        this.logger = logger;
        this.ValidationRule(x => x.Path, s => !string.IsNullOrEmpty(s), "Invalid path");
        var configurationViewModel = new ConfigurationViewModel();
        Configuration = configurationViewModel;

        var browseFolder = ReactiveCommand.CreateFromObservable(() => Observable.FromAsync(() => GetFileSystem(Configuration.Host, Configuration.Port, Configuration.Username, Configuration.Password))
            .Successes()
            .Select(folderPicker).SelectMany(picker => picker.Pick("Select a folder")), Configuration.IsValid());

        browseFolder.Values().Select(x => x.Path.ToString()).BindTo(this, x => x.Path);
        BrowseFolder = browseFolder;


        Directory = IsValid
            .Where(b => b)
            .Select(_ => this.WhenAnyValue(x => x.Path, s => s.Configuration)
                .SelectMany(tuple => GetDirectory(tuple.Item1, tuple.Item2)).Successes())
            .Switch();
    }

    public ICommand BrowseFolder { get; }

    public ConfigurationViewModel Configuration { get; }

    [Reactive] public string Path { get; set; } = "";

    public void SetProfile(IProfile profile)
    {
        var pr = (Profile) profile;
        Configuration.Host = pr.Configuration.Host;
        Configuration.Port = pr.Configuration.Port;
        Configuration.Username = pr.Configuration.Username;
        Configuration.Password = pr.Configuration.Password;
    }

    public IObservable<IZafiroDirectory> Directory { get; }
    public IObservable<bool> IsValid => this.IsValid().CombineLatest(Configuration.IsValid(), (imValid, configIsValid) => imValid && configIsValid);

    private Task<Result<IZafiroDirectory>> GetDirectory(string path, ConfigurationViewModel configuration)
    {
        return GetFileSystem(configuration.Host, configuration.Port, configuration.Username, configuration.Password).Bind(system => system.GetDirectory(path));
    }

    private async Task<Result<IFileSystem>> GetFileSystem(string host, int port, string username, string password)
    {
        var createResult = await SftpFileSystem.Create(host, port, username, password, logger);
        return createResult.Map(system => (IFileSystem) system);
    }
}