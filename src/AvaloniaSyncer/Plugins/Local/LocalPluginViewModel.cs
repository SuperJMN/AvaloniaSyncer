using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Local;

namespace AvaloniaSyncer.Plugins.Local;

public class LocalPluginViewModel : ReactiveValidationObject, IFileSystemPlugin
{
    private readonly Maybe<ILogger> logger;

    public LocalPluginViewModel(Maybe<ILogger> logger)
    {
        this.logger = logger;
        this.ValidationRule(x => x.Path, s => !string.IsNullOrEmpty(s), "Invalid path");
    }

    public Task<Result<IFileSystem>> FileSystem()
    {
        return Task.FromResult(Result.Success<IFileSystem>(new LocalFileSystem(logger)));
    }

    [Reactive] public string Path { get; set; } = "";

    public IObservable<bool> IsValid => this.IsValid();
}