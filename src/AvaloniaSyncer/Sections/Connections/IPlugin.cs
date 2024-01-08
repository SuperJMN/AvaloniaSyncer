using System;
using AvaloniaSyncer.Sections.Connections.Configuration;

namespace AvaloniaSyncer.Sections.Connections;

public interface IPlugin
{
    string Name { get; }
    IConfiguration CreateConfig(IConnectionsRepository connectionsRepository, Action<ConfigurationViewModelBase> onRemove);
}