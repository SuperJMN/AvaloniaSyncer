using System.Collections.Generic;
using AvaloniaSyncer.Plugins.SeaweedFS_new.Settings;

namespace AvaloniaSyncer.Settings;

public class Config<T>
{
    public required List<T> Profiles { get; init; }
}