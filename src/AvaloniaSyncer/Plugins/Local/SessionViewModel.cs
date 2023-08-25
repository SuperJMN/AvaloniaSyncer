using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
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

namespace AvaloniaSyncer.Plugins.Local;

public class SessionViewModel : ReactiveValidationObject, ISession
{
    private readonly Maybe<ILogger> logger;

    public SessionViewModel(Maybe<ILogger> logger)
    {
        this.logger = logger;
        this.ValidationRule(x => x.Path, s => !string.IsNullOrEmpty(s), "Invalid path");
        Configuration = Maybe<ISessionConfiguration>.None;
        Directory = this.WhenAnyValue(x => x.Path).SelectMany(s => FileSystem().Bind(f => f.GetDirectory(s))).Successes();
    }

    public Task<Result<IFileSystem>> FileSystem()
    {
        return Task.FromResult(Result.Success<IFileSystem>(new LocalFileSystem(logger)));
    }

    [Reactive] public string Path { get; set; } = "";

    public void SetProfile(IProfile profile)
    {
    }

    public IObservable<IZafiroDirectory> Directory { get; }
    public IObservable<bool> IsValid => this.IsValid();

    public Maybe<ISessionConfiguration> Configuration { get; }
}