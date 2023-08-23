using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaSyncer.Plugins;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.UI;

namespace AvaloniaSyncer.ViewModels;

public class SyncViewModel : ViewModelBase
{
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

        Syncronizations = collection;

        CreateSyncSession = ReactiveCommand.CreateFromTask(async () =>
        {
            await ShowDialog(dialogService, pluginFactories);
            Number++;
        });

        SelectPlugins = ReactiveCommand.CreateFromTask(() => dialogService.ShowDialog(new SelectPluginsViewModel(pluginFactories), "Select plugins", new OptionConfiguration<SelectPluginsViewModel>("OK", context => ReactiveCommand.Create(() =>
        {
            context.Window.Close();
            return new PluginSelection(context.ViewModel.Source!, context.ViewModel.Destination!);
        }, context.ViewModel.IsValid()))));
    }

    public ReactiveCommand<Unit, Unit> SelectPlugins { get; }

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

public record PluginSelection(IPlugin Source, IPlugin Destination);