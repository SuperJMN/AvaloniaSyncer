using System;
using AvaloniaSyncer.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.WizardOld.Interfaces;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.Synchronization.ConfigurePlugins;

public class ConfigurePluginsViewModel : ReactiveValidationObject, IValidatable
{
    private readonly ObservableAsPropertyHelper<IZafiroDirectory> destinationDirectory;
    private readonly ObservableAsPropertyHelper<IZafiroDirectory> sourceDirectory;

    public ConfigurePluginsViewModel(string name, PluginSelection selection)
    {
        Name = name;
        SourceSession = new PluginConfiguratorViewModel(selection.Source);
        DestinationSession = new PluginConfiguratorViewModel(selection.Destination);
        sourceDirectory = this.WhenAnyObservable(x => x.SourceSession.Session.Directory).ToProperty(this, x => x.SourceDirectory);
        destinationDirectory = this.WhenAnyObservable(x => x.DestinationSession.Session.Directory).ToProperty(this, x => x.DestinationDirectory);
    }

    public IZafiroDirectory DestinationDirectory => destinationDirectory.Value;

    public IZafiroDirectory SourceDirectory => sourceDirectory.Value;

    public PluginConfiguratorViewModel DestinationSession { get; }

    public PluginConfiguratorViewModel SourceSession { get; }

    public string Name { get; }

    public IObservable<bool> IsValid => this.IsValid();
}