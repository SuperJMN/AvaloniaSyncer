using System;

namespace AvaloniaSyncer.Plugins;

public interface IFileSystemPluginFactory
{
    public string Name { get; }
    public Uri Icon { get; }
    public IFileSystemPlugin Create();
}