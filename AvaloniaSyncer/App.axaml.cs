using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Markup.Xaml;
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
            return new MainViewModel(new NotificationDialog(DialogService.Create(ApplicationLifetime!, new Dictionary<Type, Type>(){ [typeof(MessageDialogViewModel)] = typeof(MessageDialogView) })), fileSystemPlugins);
        }, () => new MainWindow());
       
        base.OnFrameworkInitializationCompleted();
    }

    private static IFileSystemPluginFactory[] AvailablePlugins()
    {
        return new IFileSystemPluginFactory []
        {
            new LocalFileSystemPluginFactory(), 
            new SeaweedFileSystemPluginFactory(Maybe.From(Log.Logger)),
            new SftpPluginFactory(),
        };
    }
}