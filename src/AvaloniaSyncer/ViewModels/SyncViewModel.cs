using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Plugins;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace AvaloniaSyncer.ViewModels;

public class SyncViewModel : ViewModelBase
{
    private readonly SourceCache<SessionViewModel, string> bareSessions = new(x => x.Name);
    private readonly Maybe<ILogger> logger;
    private readonly INotificationService notificationService;
    private readonly SourceCache<SynchronizationViewModel, string> sessions = new(x => x.Title);
    private int Number = 1;

    public SyncViewModel(IDialogService dialogService, INotificationService notificationService, IList<IPlugin> pluginFactories, Maybe<ILogger> logger)
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

        Syncronizations = collection;

        CreateSyncSession = ReactiveCommand.CreateFromTask(async () =>
        {
            await ShowDialog(dialogService, pluginFactories);
            Number++;
        });

        SelectPlugins = ReactiveCommand.CreateFromTask(async () => { return await dialogService.Prompt("Select your plugins", new SelectPluginsViewModel(pluginFactories), "OK", vm => ReactiveCommand.Create(() => new PluginSelection(vm.Source!, vm.Destination!), vm.IsValid())); });

        SelectPlugins.Values().Do(selection => bareSessions.AddOrUpdate(new SessionViewModel("Session", selection))).Subscribe();
    }

    public ReadOnlyObservableCollection<SessionViewModel> BareSessions { get; set; }

    public ReactiveCommand<Unit, Maybe<PluginSelection>> SelectPlugins { get; }

    public ReactiveCommand<Unit, Unit> CreateSyncSession { get; set; }


    public IEnumerable<SynchronizationViewModel> Syncronizations { get; }

    private Task ShowDialog(IDialogService dialogService, IList<IPlugin> pluginFactories)
    {
        var vm = new CreateSyncSessionViewModel(pluginFactories);

        var options = new[]
        {
            new OptionConfiguration<CreateSyncSessionViewModel>("Cancel", actionContext => ReactiveCommand.Create(() => actionContext.Window.Close())),
            new OptionConfiguration<CreateSyncSessionViewModel>("Create", actionContext =>
            {
                return vm.CreateSession.Extend(result =>
                {
                    result.Tap(tuple =>
                    {
                        sessions.AddOrUpdate(new SynchronizationViewModel($"Session {Number++}", notificationService, tuple.Source, tuple.Destination, logger));
                        actionContext.Window.Close();
                    });
                });
            })
        };

        return dialogService.ShowDialog(vm, "Create  sync session", options);
    }
}

public class SessionViewModel
{
    public SessionViewModel(string name, PluginSelection selection)
    {
        Name = name;
        Selection = selection;
    }

    public string Name { get; }
    public PluginSelection Selection { get; }
}

public record PluginSelection(IPlugin Source, IPlugin Destination);