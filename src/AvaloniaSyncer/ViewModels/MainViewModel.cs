using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Plugins;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace AvaloniaSyncer.ViewModels;

public class MainViewModel : ViewModelBase
{
    private int Number = 1;
    private Maybe<ILogger> logger;

    public MainViewModel(INotificationService notificationService, IList<IFileSystemPluginFactory> pluginFactories, Maybe<ILogger> logger)
    {
        this.logger = logger;
        SourcePluginViewModel = new PluginSelectionViewModel("Source", pluginFactories);
        DestinationPluginViewModel = new PluginSelectionViewModel("Destination", pluginFactories);

        var canCreateSession = this
            .WhenAnyObservable(x => x.SourcePluginViewModel.SelectedPlugin.IsValid)
            .CombineLatest(this.WhenAnyObservable(x => x.DestinationPluginViewModel.SelectedPlugin.IsValid), (isSrcValid, isDestValid) => isSrcValid && isDestValid);

        var createSession = ReactiveCommand
            .CreateFromTask(() => CreateSyncronizationSession(notificationService, SourcePluginViewModel.SelectedPlugin!, DestinationPluginViewModel.SelectedPlugin!),
                canCreateSession);

        createSession
            .Successes()
            .ToObservableChangeSet()
            .StartWith()
            .Bind(out var collection)
            .Subscribe();

        Syncronizations = collection;
        
        AddSession = createSession;
        AddSession.HandleErrorsWith(notificationService);
    }

    public ReactiveCommand<Unit, Result<SynchronizationViewModel>> AddSession { get; set; }

    public PluginSelectionViewModel DestinationPluginViewModel { get; }

    public PluginSelectionViewModel SourcePluginViewModel { get; }

    public IEnumerable<SynchronizationViewModel> Syncronizations { get; }


    private Task<Result<SynchronizationViewModel>> CreateSyncronizationSession(INotificationService myNotificationService, IFileSystemPlugin sourcePlugin, IFileSystemPlugin destination)
    {
        var sourceDirResult = sourcePlugin.FileSystem().Bind(fs => fs.GetDirectory(sourcePlugin.Path));
        var destDirResult = destination.FileSystem().Bind(fs => fs.GetDirectory(destination.Path));
        return sourceDirResult.CombineAndMap(destDirResult, (a, b) => new SynchronizationViewModel($"Session {Number++}", myNotificationService, a, b, logger));
    }
}