using System;
using AvaloniaSyncer.Plugins.Local;
using CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Plugins;

public interface IPlugin
{
    public string Name { get; }
    public Uri Icon { get; }
    public ISession Create();
    public Maybe<IPluginSettings> Settings { get; }
}