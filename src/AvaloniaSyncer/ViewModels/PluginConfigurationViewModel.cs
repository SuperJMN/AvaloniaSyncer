using AvaloniaSyncer.Plugins;
using CSharpFunctionalExtensions;

namespace AvaloniaSyncer.ViewModels;

public class PluginConfigurationViewModel : ViewModelBase
{
    public PluginConfigurationViewModel(string name, Maybe<IPluginSettings> configuration)
    {
        Name = name;
        Configuration = configuration;
        Configuration.Execute(config => config.Load.Execute(null));
    }

    public Maybe<IPluginSettings> Configuration { get; }

    public string Name { get; }
}