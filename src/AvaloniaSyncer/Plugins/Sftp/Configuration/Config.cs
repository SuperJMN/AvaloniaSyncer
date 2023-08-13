using System.Collections.Generic;

namespace AvaloniaSyncer.Plugins.Sftp.Configuration;

public class Config
{
    public required List<ProfileDto> Profiles { get; init; }
}