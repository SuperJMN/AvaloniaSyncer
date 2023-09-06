using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using AvaloniaSyncer.Plugins;
using AvaloniaSyncer.Sections.Synchronization.ConfigurePlugins;
using AvaloniaSyncer.Sections.Synchronization.Sync;
using AvaloniaSyncer.Sections.Synchronize.SelectPlugins;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Synchronization;

public class SyncSectionViewModel : ViewModelBase
{
    private readonly SourceCache<ConfigurePluginsViewModel, string> bareSessions = new(x => x.Name);
    private readonly Maybe<ILogger> logger;
    private readonly INotificationService notificationService;
    private readonly SourceCache<SynchronizationViewModel, string> sessions = new(x => x.Title);
    private int Number = 1;

    public SyncSectionViewModel(IDialogService dialogService, INotificationService notificationService, IList<IPlugin> pluginFactories, Maybe<ILogger> logger)
    {
        this.notificationService = notificationService;
        this.logger = logger;

        sessions
            .Connect()
            .Bind(out var collection)
            .Subscribe();

        bareSessions
            .Connect()
            .Bind(out var bareSessionsCollection)
            .Subscribe();

        BareSessions = bareSessionsCollection;

        SelectPlugins = ReactiveCommand.CreateFromTask(async () => { return await dialogService.Prompt("Select your plugins", new SelectPluginsViewModel(pluginFactories), "OK", vm => ReactiveCommand.Create(() => new PluginSelection(vm.Source!, vm.Destination!), vm.IsValid())); });

        SelectPlugins.Values().Do(selection => bareSessions.AddOrUpdate(new ConfigurePluginsViewModel("Session", selection))).Subscribe();
    }

    public ReadOnlyObservableCollection<ConfigurePluginsViewModel> BareSessions { get; set; }

    public ReactiveCommand<Unit, Maybe<PluginSelection>> SelectPlugins { get; }
}