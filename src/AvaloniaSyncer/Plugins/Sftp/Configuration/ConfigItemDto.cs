using System;

namespace AvaloniaSyncer.Plugins.Sftp.Configuration;

public class ConfigItemDto
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public Guid Id { get; set; }
}