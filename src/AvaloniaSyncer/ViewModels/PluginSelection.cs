using AvaloniaSyncer.Plugins;

namespace AvaloniaSyncer.ViewModels;

public record PluginSelection(IPlugin Source, IPlugin Destination);