using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Local;

namespace AvaloniaSyncer.Plugins.Local;

public class LocalPlugin : ReactiveValidationObject, IFileSystemPlugin
{
    public Task<Result<IFileSystem>> FileSystem() => Task.FromResult(Result.Success<IFileSystem>(new LocalFileSystem()));

    [Reactive]
    public string Path { get; set; } = "";

    public LocalPlugin()
    {
        this.ValidationRule(x => x.Path, s => !string.IsNullOrEmpty(s), "Invalid path");
    }

    public IObservable<bool> IsValid => this.IsValid();
}