using System.Windows.Input;

namespace AvaloniaSyncer.Plugins.Local;

public interface IPluginSettings
{
    ICommand Load { get; }
}