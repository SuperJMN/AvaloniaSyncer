using System;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization.Model;

public class Connection
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ConnectionParameters Parameters { get; set; }
}