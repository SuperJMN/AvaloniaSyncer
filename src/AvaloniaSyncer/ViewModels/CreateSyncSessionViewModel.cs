using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Plugins;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.CSharpFunctionalExtensions;

namespace AvaloniaSyncer.ViewModels;

public class CreateSyncSessionViewModel
{
    public CreateSyncSessionViewModel(IList<IPlugin> pluginFactories)
    {
        SourcePluginViewModel = new PluginSelectionViewModel("Source", pluginFactories);
        DestinationPluginViewModel = new PluginSelectionViewModel("Destination", pluginFactories);
        var canCreateSession = this
            .WhenAnyObservable(x => x.SourcePluginViewModel.SelectedPlugin.IsValid)
            .CombineLatest(this.WhenAnyObservable(x => x.DestinationPluginViewModel.SelectedPlugin.IsValid), (isSrcValid, isDestValid) => isSrcValid && isDestValid);

        var createSession = ReactiveCommand
            .CreateFromObservable(() => Observable.FromAsync(() => CreateSyncSession(SourcePluginViewModel.SelectedPlugin!, DestinationPluginViewModel.SelectedPlugin!)),
                canCreateSession);
        CreateSession = createSession;
        ErrorMessages = CreateSession.Failures();
        IsBusy = CreateSession.IsExecuting;
    }

    public IObservable<bool> IsBusy { get; }

    public IObservable<string> ErrorMessages { get; }

    public ReactiveCommand<Unit, Result<SyncSession>> CreateSession { get; }

    public PluginSelectionViewModel DestinationPluginViewModel { get; }

    public PluginSelectionViewModel SourcePluginViewModel { get; }

    private Task<Result<SyncSession>> CreateSyncSession(ISession sourcePlugin, ISession destination)
    {
        var sourceDirResult = sourcePlugin.FileSystem().Bind(fs => fs.GetDirectory(sourcePlugin.Path));
        var destDirResult = destination.FileSystem().Bind(fs => fs.GetDirectory(destination.Path));
        return sourceDirResult.CombineAndMap(destDirResult, (source, dest) => new SyncSession(source, dest));
    }
}