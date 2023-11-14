using System;
using AvaloniaSyncer.Controls;

namespace AvaloniaSyncer.Sections.Connections;

public interface IConfiguration
{
    public Guid Id { get; }
    IObservable<bool> IsValid { get; }
    public StringProperty Name { get; }
}