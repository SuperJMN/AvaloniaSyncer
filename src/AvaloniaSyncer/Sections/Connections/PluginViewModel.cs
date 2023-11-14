using System;
using System.Reactive;
using DynamicData;
using ReactiveUI;

namespace AvaloniaSyncer.Sections.Connections;

public class PluginViewModel : ReactiveObject
{
    public PluginViewModel(IPlugin plugin, SourceCache<IConfiguration, Guid> sourceCache, IConnectionsRepository connectionsRepository)
    {
        Add = ReactiveCommand.Create(() => sourceCache.AddOrUpdate(plugin.CreateConfig(connectionsRepository)));
        Name = plugin.Name;
    }

    public ReactiveCommand<Unit, Unit> Add { get; }

    public string Name { get; }
}