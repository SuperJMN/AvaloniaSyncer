using System.Collections.Generic;
using System.Linq;
using AvaloniaSyncer.Plugins;

namespace AvaloniaSyncer.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel(IEnumerable<IPlugin> pluginFactories)
    {
        Configurations = pluginFactories.Select(c => new PluginConfigurationViewModel(c.Name, c.Settings)).ToList();
    }

    public List<PluginConfigurationViewModel> Configurations { get; }
}