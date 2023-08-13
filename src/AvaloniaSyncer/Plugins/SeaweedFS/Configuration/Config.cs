using System.Collections.Generic;

namespace AvaloniaSyncer.Plugins.SeaweedFS.Configuration;

public class Config
{
    public required List<ProfileDto> Profiles { get; init; }
}