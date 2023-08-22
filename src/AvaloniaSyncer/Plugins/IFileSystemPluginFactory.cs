using System;
using AvaloniaSyncer.Plugins.Local;
using CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Plugins;

public interface IFileSystemPluginFactory
{
    public string Name { get; }
    public Uri Icon { get; }
    public IFileSystemPlugin Create();
    public Maybe<IPluginSettings> Settings { get; }
}