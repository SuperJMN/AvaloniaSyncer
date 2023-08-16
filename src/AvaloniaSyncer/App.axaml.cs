using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Markup.Xaml;
using AvaloniaSyncer.Plugins;
using AvaloniaSyncer.Plugins.Local;
using AvaloniaSyncer.Plugins.SeaweedFS;
using AvaloniaSyncer.Plugins.Sftp;
using AvaloniaSyncer.ViewModels;
using AvaloniaSyncer.Views;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Mixins;

namespace AvaloniaSyncer;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        AppDomain.CurrentDomain.UnhandledException += (sender, args) => Log.Fatal(args.ExceptionObject.ToString());
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        this.Connect(() => new MainView(), control =>
        {
            var fileSystemPlugins = AvailablePlugins();
            var dialogService = DialogService.Create(ApplicationLifetime!, new Dictionary<Type, Type>
            {
                [typeof(MessageDialogViewModel)] = typeof(MessageDialogView),
                [typeof(CreateSyncSessionViewModel)] = typeof(CreateSyncSessionView),
            }, configureWindow: Maybe<Action<ConfigureWindowContext>>.From(ConfigureWindow));
            var notificationService = new NotificationDialog(dialogService);
            return new MainViewModel(dialogService, notificationService, fileSystemPlugins, Maybe.From(Log.Logger));
        }, () => new MainWindow());

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureWindow(ConfigureWindowContext context)
    {
        context.ToConfigure.Width = context.Parent.Bounds.Width / 2;
        context.ToConfigure.Height = context.Parent.Bounds.Height / 2;
    }

    private static IFileSystemPluginFactory[] AvailablePlugins()
    {
        var logger = Maybe.From(Log.Logger);
        return new IFileSystemPluginFactory[]
        {
            new LocalFileSystemPluginFactory(logger),
            new SeaweedFileSystemPluginFactory(logger),
            new SftpPluginFactory(logger),
        };
    }
}