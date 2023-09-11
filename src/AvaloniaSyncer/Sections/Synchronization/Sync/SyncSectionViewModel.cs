using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using AvaloniaSyncer.Plugins;
using AvaloniaSyncer.Sections.Synchronization.ConfigurePlugins;
using AvaloniaSyncer.Sections.Synchronization.SelectPlugins;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Wizard;
using Zafiro.Avalonia.Wizard.Interfaces;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Synchronization.Sync;

public class SyncSectionViewModel : ViewModelBase
{
    private readonly SourceCache<SynchronizationViewModel, string> sessions = new(x => x.Title);
    private int Number = 1;

    public SyncSectionViewModel(IDialogService dialogService, IEnumerable<IPlugin> plugins, INotificationService notificationService, Maybe<ILogger> logger)
    {
        sessions
            .Connect()
            .Bind(out var collection)
            .Subscribe();

        Sessions = collection;

        SelectPlugins = ReactiveCommand.CreateFromTask(async () =>
        {
            IList<IPage<IValidatable, IValidatable>> pages = new List<IPage<IValidatable, IValidatable>>()
            {
                new Page<IValidatable, IValidatable>(_ => new SelectPluginsViewModel(plugins), "Next"),
                new Page<IValidatable, IValidatable>(previous =>
                {
                    var selectVm = (SelectPluginsViewModel) previous;
                    return new ConfigurePluginsViewModel("Session", new PluginSelection(selectVm.Source!, selectVm.Destination!));
                }, "Next"),
            };

            var wizardViewModel = new Wizard<ConfigurePluginsViewModel>(pages);

            var showDialog = await dialogService.ShowDialog(wizardViewModel, "Create a synchronization session", wizard => Observable.FromAsync(() => wizard.Result));
            return showDialog.Map(x => new SyncSession(x.SourceDirectory, x.DestinationDirectory));
        });

        SelectPlugins.Values().Do(session =>
        {
            sessions.AddOrUpdate(new SynchronizationViewModel("Session", notificationService, session.Source, session.Destination, logger));
        }).Subscribe();
    }

    private static Func<Wizard<ConfigurePluginsViewModel>, IObservable<SyncSession>> Results()
    {
        return model => Observable.FromAsync(() => model.Result)
            .Select(session => new SyncSession(session.SourceDirectory!, session.DestinationDirectory!));
    }

    public ReadOnlyObservableCollection<SynchronizationViewModel> Sessions { get; set; }

    public ReactiveCommand<Unit, Maybe<SyncSession>> SelectPlugins { get; }
}