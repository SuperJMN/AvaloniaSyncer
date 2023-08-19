using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaSyncer.ViewModels;
using AvaloniaSyncer.Views;
using JetBrains.Annotations;
using Renci.SshNet;
using Serilog;
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
        this.Connect(() => new MainView(), _ => new MainViewModel(), () => new MainWindow());

        base.OnFrameworkInitializationCompleted();
    }
}