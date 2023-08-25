using System;
using AvaloniaSyncer.Settings;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Plugins;

public interface ISession
{
    public IObservable<IZafiroDirectory> Directory { get; }
    IObservable<bool> IsValid { get; }
    public string Path { get; set; }
    void SetProfile(IProfile profile);
}