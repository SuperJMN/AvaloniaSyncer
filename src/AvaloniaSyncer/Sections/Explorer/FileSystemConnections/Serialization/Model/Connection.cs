using System.Text.Json.Serialization;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization.Model;

public class Connection
{
    public string Name { get; set; }
    public ConnectionParameters Parameters { get; set; }
}