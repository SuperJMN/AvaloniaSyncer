﻿using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using AvaloniaSyncer.Plugins;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.Pickers;
using Zafiro.Avalonia.Notifications;

namespace AvaloniaSyncer;

public class ViewModelFactory
{
    public ViewModelFactory(IApplicationLifetime applicationLifetime, Visual control)
    {
        NotificationService = new NotificationService(new WindowNotificationManager(TopLevel.GetTopLevel(control)));
        DialogService = Zafiro.Avalonia.Dialogs.DialogService.Create(applicationLifetime, new Dictionary<Type, Type>(), configureWindow: Maybe<Action<ConfigureWindowContext>>.From(ConfigureWindow));
        Plugins = AvailablePlugins();
    }

    public NotificationService NotificationService { get; set; }

    private IPlugin[] Plugins { get; }

    private IDialogService DialogService { get; }

    public SyncSectionViewModel GetSyncViewModel()
    {
        var notificationService = new NotificationDialog(DialogService);
        return new SyncSectionViewModel(DialogService, notificationService, Plugins, Maybe.From(Log.Logger));
    }

    private static void ConfigureWindow(ConfigureWindowContext context)
    {
        context.ToConfigure.Width = context.Parent.Bounds.Width / 1.5;
        context.ToConfigure.Height = context.Parent.Bounds.Height / 1.5;
    }

    private IPlugin[] AvailablePlugins()
    {
        var logger = Maybe.From(Log.Logger);
        return new IPlugin[]
        {
            new Plugins.Local.Plugin(system => new FolderPicker(DialogService, system, NotificationService), logger),
            new Plugins.SeaweedFS.Plugin(system => new FolderPicker(DialogService, system, NotificationService), logger),
            new Plugins.Sftp.Plugin(logger)
        };
    }

    public SettingsViewModel GetSettingsViewModel()
    {
        return new SettingsViewModel(Plugins);
    }
}