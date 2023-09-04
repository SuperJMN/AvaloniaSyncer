using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaSyncer.Settings;
using CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Plugins;

public interface IPluginSettings
{
    ICommand Load { get; }
    Task<Maybe<IEnumerable<IProfile>>> GetProfiles();
}