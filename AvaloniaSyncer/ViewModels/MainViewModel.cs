using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using Zafiro.Functional;
using Zafiro.UI;

namespace AvaloniaSyncer.ViewModels;

public class MainViewModel : ViewModelBase
{
    private int Number = 1;

    public MainViewModel(INotificationService notificationService, IList<IFileSystemPluginFactory> pluginFactories)
    {
        SourcePluginViewModel = new PluginSelectionViewModel("Source", pluginFactories);
        DestinationPluginViewModel = new PluginSelectionViewModel("Destination", pluginFactories);

        var createSession = ReactiveCommand
            .CreateFromObservable(() => CreateSyncronizationSession(notificationService, SourcePluginViewModel.SelectedPlugin!, DestinationPluginViewModel.SelectedPlugin!),
                this.WhenAnyValue(x => x.SourcePluginViewModel.SelectedPlugin, x => x.DestinationPluginViewModel.SelectedPlugin, (src, dst) => src != null && dst != null));

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


    private IObservable<Result<SynchronizationViewModel>> CreateSyncronizationSession(INotificationService myNotificationService, IFileSystemPlugin sourcePlugin, IFileSystemPlugin destination)
    {
        var getOrigin = () => sourcePlugin.FileSystem().Map(fs => fs.GetDirectory(sourcePlugin.Path));
        var getDestination = () => destination.FileSystem().Map(fs => fs.GetDirectory(destination.Path));
        var r = FunctionalMixin.Combine(() => getOrigin(), () => getDestination(), (a, b) => new SynchronizationViewModel($"Session {Number++}", myNotificationService, a, b));
        return r;
    }
}