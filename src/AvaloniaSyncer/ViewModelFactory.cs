using System;
using System.Collections.Generic;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaSyncer.Plugins;
using AvaloniaSyncer.Plugins.Local;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Avalonia.Dialogs;

namespace AvaloniaSyncer;

public static class ViewModelFactory
{
    public static SyncViewModel GetSyncViewModel(IApplicationLifetime applicationLifetime)
    {
        var fileSystemPlugins = AvailablePlugins();
        var dialogService = DialogService.Create(applicationLifetime, new Dictionary<Type, Type>
        {
            [typeof(MessageDialogViewModel)] = typeof(MessageDialogView),
        }, configureWindow: Maybe<Action<ConfigureWindowContext>>.From(ConfigureWindow));
        var notificationService = new NotificationDialog(dialogService);
        return new SyncViewModel(dialogService, notificationService, fileSystemPlugins, Maybe.From(Log.Logger));
    }

    private static void ConfigureWindow(ConfigureWindowContext context)
    {
        context.ToConfigure.Width = context.Parent.Bounds.Width / 1.5;
        context.ToConfigure.Height = context.Parent.Bounds.Height / 1.5;
    }

    private static IPlugin[] AvailablePlugins()
    {
        var logger = Maybe.From(Log.Logger);
        return new IPlugin[]
        {
            new PluginViewModel(logger),
            new AvaloniaSyncer.Plugins.SeaweedFS_new.PluginViewModel(logger),
            //new SftpPlugin(logger)
        };
    }

    public static SettingsViewModel GetSettingsViewModel()
    {
        return new SettingsViewModel(AvailablePlugins());
    }
}