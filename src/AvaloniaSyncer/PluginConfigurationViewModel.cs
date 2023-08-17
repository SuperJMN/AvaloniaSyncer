using AvaloniaSyncer.Plugins.Local;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;

namespace AvaloniaSyncer;

public class PluginConfigurationViewModel : ViewModelBase
{
    public PluginConfigurationViewModel(string name, Maybe<IPluginConfiguration> configuration)
    {
        Name = name;
        Configuration = configuration;
        Configuration.Execute(config => config.Load.Execute(null));
    }

    public Maybe<IPluginConfiguration> Configuration { get; }

    public string Name { get; }
}