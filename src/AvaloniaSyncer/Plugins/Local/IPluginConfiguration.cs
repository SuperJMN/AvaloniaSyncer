using System.Windows.Input;

namespace AvaloniaSyncer.Plugins.Local;

public interface IPluginConfiguration
{
    ICommand Load { get; }
}