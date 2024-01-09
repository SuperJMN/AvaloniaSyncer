using System;
using System.Reactive;
using AvaloniaSyncer.Sections.Connections.Configuration;
using DynamicData;
using ReactiveUI;

namespace AvaloniaSyncer.Sections.Connections;

public class PluginViewModel : ReactiveObject
{
    public PluginViewModel(IPlugin plugin, ISourceCache<IConfiguration, Guid> sourceCache, IConnectionsRepository connectionsRepository, Action<ConfigurationViewModelBase> onRemove)
    {
        Add = ReactiveCommand.Create(() => sourceCache.AddOrUpdate(plugin.CreateConfig(connectionsRepository, onRemove)));
        Name = plugin.Name;
    }

    public ReactiveCommand<Unit, Unit> Add { get; }

    public string Name { get; }
}