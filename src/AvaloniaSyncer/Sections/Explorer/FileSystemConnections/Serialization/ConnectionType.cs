using System.Text.Json.Serialization;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConnectionType
{
    Local,
    SeaweedFS,
    Sftp
}