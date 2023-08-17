using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaSyncer.Plugins;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace AvaloniaSyncer.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly Maybe<ILogger> logger;
    private readonly INotificationService notificationService;
    private readonly SourceCache<SynchronizationViewModel, string> sessions = new(x => x.Title);
    private int Number = 1;

    public MainViewModel(IDialogService dialogService, INotificationService notificationService, IList<IFileSystemPluginFactory> pluginFactories, Maybe<ILogger> logger)
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
    }

    public ReactiveCommand<Unit, Unit> CreateSyncSession { get; set; }


    public IEnumerable<SynchronizationViewModel> Syncronizations { get; }

    private Task ShowDialog(IDialogService dialogService, IList<IFileSystemPluginFactory> pluginFactories)
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