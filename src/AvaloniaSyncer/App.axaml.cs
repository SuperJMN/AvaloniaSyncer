using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Markup.Xaml;
using AvaloniaSyncer.ViewModels;
using AvaloniaSyncer.Views;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Avalonia.Mixins;

namespace AvaloniaSyncer;

public class App : Application
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
        this.Connect(() => new MainView(), view =>
        {
            var vm = new ViewModelFactory(ApplicationLifetime!, view, Maybe<ILogger>.From(Log.Logger));

           

            return new MainViewModel(async () =>
            {
                var sections = new List<Section>
                {
                    new("Explore", vm.GetExploreSection()),
                    new("Synchronize", vm.GetSynchronizationSection()),
                    new("Settings", await vm.GetSettingsViewModel2())
                };

                return sections;
            });
        }, () => new MainWindow());

        base.OnFrameworkInitializationCompleted();
    }
}