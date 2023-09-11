using System;
using System.Collections.Generic;
using AvaloniaSyncer.Plugins;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Misc;
using Zafiro.Avalonia.Wizard.Interfaces;

namespace AvaloniaSyncer.Sections.Synchronization.SelectPlugins;

public class SelectPluginsViewModel : ReactiveValidationObject, IValidatable, IViewModel
{
    public IEnumerable<IPlugin> Plugins { get; }

    public SelectPluginsViewModel(IEnumerable<IPlugin> plugins)
    {
        Plugins = plugins;
        this.ValidationRule(x => x.Source, x => x is not null, "Can't be empty");
        this.ValidationRule(x => x.Destination, x => x is not null, "Can't be empty");
    }

    [Reactive] public IPlugin? Source { get; set; }
    [Reactive] public IPlugin? Destination { get; set; }
    public IObservable<bool> IsValid => this.IsValid();
}