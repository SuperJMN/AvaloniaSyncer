using System.Collections.Generic;
using System.Linq;
using AvaloniaSyncer.Plugins;
using AvaloniaSyncer.ViewModels;

namespace AvaloniaSyncer.Sections.Settings;

public class SettingsSectionViewModel : ViewModelBase
{
    public SettingsSectionViewModel(IEnumerable<IPlugin> plugins)
    {
        Configurations = plugins.Select(c => new PluginConfigurationViewModel(c.Name, c.Settings)).ToList();
    }

    public List<PluginConfigurationViewModel> Configurations { get; }
}