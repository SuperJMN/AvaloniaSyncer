﻿using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Markup.Xaml;
using AvaloniaSyncer.ViewModels;
using AvaloniaSyncer.Views;
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
            var vm = new ViewModelFactory(ApplicationLifetime!, view);

            var sections = new List<Section>
            {
                new("Synchronize", vm.GetSyncViewModel()),
                new("Settings", vm.GetSettingsViewModel()),
            };

            return new MainViewModel(sections);
        }, () => new MainWindow());

        base.OnFrameworkInitializationCompleted();
    }
}