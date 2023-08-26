using System;

namespace AvaloniaSyncer.Plugins.Sftp.Settings;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}