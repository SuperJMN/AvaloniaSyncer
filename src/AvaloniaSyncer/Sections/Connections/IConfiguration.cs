using System;

namespace AvaloniaSyncer.Sections.Connections;

public interface IConfiguration
{
    public string Name { get; }
    IObservable<bool> IsValid { get; }
}