﻿using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Plugins;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.ViewModels;

public class CreateSyncSessionViewModel
{
    public CreateSyncSessionViewModel(IList<IFileSystemPluginFactory> pluginFactories)
    {
        SourcePluginViewModel = new PluginSelectionViewModel("Source", pluginFactories);
        DestinationPluginViewModel = new PluginSelectionViewModel("Destination", pluginFactories);
        var canCreateSession = this
            .WhenAnyObservable(x => x.SourcePluginViewModel.SelectedPlugin.IsValid)
            .CombineLatest(this.WhenAnyObservable(x => x.DestinationPluginViewModel.SelectedPlugin.IsValid), (isSrcValid, isDestValid) => isSrcValid && isDestValid);

        var createSession = ReactiveCommand
            .CreateFromTask(() => CreateSyncSession(SourcePluginViewModel.SelectedPlugin!, DestinationPluginViewModel.SelectedPlugin!),
                canCreateSession);
        CreateSession = createSession;
    }

    public ReactiveCommand<Unit, Result<(IZafiroDirectory, IZafiroDirectory)>> CreateSession { get; }

    private Task<Result<(IZafiroDirectory, IZafiroDirectory)>> CreateSyncSession(IFileSystemPlugin sourcePlugin, IFileSystemPlugin destination)
    {
        var sourceDirResult = sourcePlugin.FileSystem().Bind(fs => fs.GetDirectory(sourcePlugin.Path));
        var destDirResult = destination.FileSystem().Bind(fs => fs.GetDirectory(destination.Path));
        return sourceDirResult.CombineAndMap(destDirResult, (source, dest) => (directory: source, zafiroDirectory: dest));
    }

    public PluginSelectionViewModel DestinationPluginViewModel { get; }

    public PluginSelectionViewModel SourcePluginViewModel { get; }
}