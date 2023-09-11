using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using AvaloniaSyncer.Plugins;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Wizard.Interfaces;

namespace AvaloniaSyncer.ViewModels;

public class PluginSelectionViewModel : ReactiveValidationObject, IValidatable
{
    private readonly ObservableAsPropertyHelper<ISession?> sourcePlugin;

    public PluginSelectionViewModel(string name, IEnumerable<IPlugin> pluginFactories)
    {
        Name = name;
        PluginFactories = pluginFactories;

        sourcePlugin = this
            .WhenAnyValue(x => x.SelectedPluginFactory)
            .WhereNotNull()
            .Select(x => x.Create())
            .ToProperty(this, model => model.SelectedPlugin);
    }


    public ISession? SelectedPlugin => sourcePlugin.Value;

    [Reactive] public IPlugin? SelectedPluginFactory { get; set; }

    public string Name { get; }

    public IEnumerable<IPlugin> PluginFactories { get; set; }
    public IObservable<bool> IsValid => this.IsValid();
}