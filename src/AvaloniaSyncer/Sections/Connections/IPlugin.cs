﻿namespace AvaloniaSyncer.Sections.Connections;

public interface IPlugin
{
    string Name { get; }
    IConfiguration CreateConfig(IConnectionsRepository connectionsRepository);
}