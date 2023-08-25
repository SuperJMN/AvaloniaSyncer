using System;

namespace AvaloniaSyncer.Settings;

public interface IProfile
{
    public Guid Id { get; }
    public string Name { get; set; }
    public IObservable<bool> IsValid { get; }
}