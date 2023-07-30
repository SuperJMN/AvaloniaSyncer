using System.Collections.Generic;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AvaloniaSyncer.ViewModels;

public class PluginSelectionViewModel : ViewModelBase
{
    private readonly ObservableAsPropertyHelper<IFileSystemPlugin?> sourcePlugin;

    public PluginSelectionViewModel(string name, IEnumerable<IFileSystemPluginFactory> pluginFactories)
    {
        Name = name;
        PluginFactories = pluginFactories;

        sourcePlugin = this
            .WhenAnyValue(x => x.SelectedPluginFactory)
            .WhereNotNull()
            .Select(x => x.Create())
            .ToProperty(this, model => model.SelectedPlugin);
    }


    public IFileSystemPlugin? SelectedPlugin => sourcePlugin.Value;

    [Reactive] public IFileSystemPluginFactory? SelectedPluginFactory { get; set; }

    public string Name { get; }
    public IEnumerable<IFileSystemPluginFactory> PluginFactories { get; set; }
}