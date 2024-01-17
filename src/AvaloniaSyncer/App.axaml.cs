using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Markup.Xaml;
using AvaloniaSyncer.ViewModels;
using AvaloniaSyncer.Views;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Avalonia.Mixins;
using Zafiro.UI;

namespace AvaloniaSyncer;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        AppDomain.CurrentDomain.UnhandledException += (sender, args) => Log.Fatal(args.ExceptionObject.ToString());

#if DEBUG
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        this.Connect(() => new MainView(), view =>
        {
            var vm = new ViewModelFactory(ApplicationLifetime!, view, new OperatingSystemContentOpener(), Maybe<ILogger>.From(Log.Logger));

            return new MainViewModel(async () =>
            {
                var sections = new List<Section>
                {
                    new("Explore", await vm.GetExploreSection()),
                    new("Synchronize", await vm.GetSynchronizationSection()),
                    new("Settings", await vm.GetConnectionsViewModel())
                };

                return sections;
            }, vm.TransferManager);
        }, () => new MainWindow());

        base.OnFrameworkInitializationCompleted();
    }
}