using System.Windows.Input;

namespace AvaloniaSyncer.Plugins;

public interface IPluginSettings
{
    ICommand Load { get; }
}