using System;
using System.Reactive.Linq;
using AvaloniaSyncer.Settings;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using Zafiro.Avalonia.FileExplorer.ViewModels;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Local;

namespace AvaloniaSyncer.Plugins.Local;

public class SessionViewModel : ReactiveValidationObject, ISession
{
    public SessionViewModel(Maybe<ILogger> logger)
    {
        this.ValidationRule(x => x.Path, s => !string.IsNullOrEmpty(s), "Invalid path");
        Configuration = Maybe<ISessionConfiguration>.None;
        var fileSystem = new LocalFileSystem(logger);
        Directory = this.WhenAnyValue(x => x.FileExplorerViewModel.Path).SelectMany(s => fileSystem.GetDirectory(s)).Successes();
        FileExplorerViewModel = new FolderContentsViewModel(fileSystem);
    }

    public Maybe<ISessionConfiguration> Configuration { get; }
    public FolderContentsViewModel FileExplorerViewModel { get; }

    [Reactive] public string Path { get; set; } = "";

    public void SetProfile(IProfile profile)
    {
    }

    public IObservable<IZafiroDirectory> Directory { get; }
    public IObservable<bool> IsValid => this.IsValid();
}