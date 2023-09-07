using System.Collections.Generic;
using AvaloniaSyncer.Plugins;
using AvaloniaSyncer.Sections.Synchronization.ConfigurePlugins;
using AvaloniaSyncer.Sections.Synchronize.SelectPlugins;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Avalonia.Model;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Synchronization;

public class WizardViewModel : ViewModelBase
{
    public WizardViewModel(IEnumerable<IPlugin> plugins, INotificationService notificationService, Maybe<ILogger> logger)
    {
        var pages = PageBuilder
            .PageFor(() => new SelectPluginsViewModel(plugins), "Next")
            .WithNext(previous => new ConfigurePluginsViewModel("Session", new PluginSelection(previous.Source!, previous.Destination!)), "Next")
            //.WithNext(previous => new Sync.SynchronizationViewModel("Session", notificationService, previous.SourceDirectory, previous.DestinationDirectory, logger), "")
            .Build();
        Wizard = new Wizard(pages);
    }

    public Wizard Wizard { get; }
}