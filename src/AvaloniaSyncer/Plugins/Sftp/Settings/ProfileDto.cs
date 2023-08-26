using System;

namespace AvaloniaSyncer.Plugins.Sftp.Settings;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}