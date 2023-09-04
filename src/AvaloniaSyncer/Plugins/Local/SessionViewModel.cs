using System;
using System.Reactive.Linq;
using System.Windows.Input;
using AvaloniaSyncer.Settings;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Local;
using Zafiro.UI;

namespace AvaloniaSyncer.Plugins.Local;

public class SessionViewModel : ReactiveValidationObject, ISession
{
    public SessionViewModel(Func<IFileSystem, IFolderPicker> folderPicker, Maybe<ILogger> logger)
    {
        this.ValidationRule(x => x.Path, s => !string.IsNullOrEmpty(s), "Invalid path");
        Configuration = Maybe<ISessionConfiguration>.None;
        var fileSystem = new LocalFileSystem(logger);
        var browseFolder = ReactiveCommand.CreateFromObservable(() => folderPicker(fileSystem).Pick("Select a folder"));
        browseFolder.Values().Select(x => x.Path.ToString()).BindTo(this, x => x.Path);
        BrowseFolder = browseFolder;
        Directory = this
            .WhenAnyValue(x => x.Path).Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
            .SelectMany(s => fileSystem.GetDirectory(s))
            .Successes();
    }

    public Maybe<ISessionConfiguration> Configuration { get; }

    [Reactive] public string Path { get; set; } = "";

    public void SetProfile(IProfile profile)
    {
    }

    public IObservable<IZafiroDirectory> Directory { get; }
    public IObservable<bool> IsValid => this.IsValid();
    public ICommand BrowseFolder { get; }
}