using System.Text.Json.Serialization;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization.Model;

[JsonDerivedType(typeof(Local), nameof(Local))]
[JsonDerivedType(typeof(SeaweedFS), nameof(SeaweedFS))]
[JsonDerivedType(typeof(Sftp), nameof(Sftp))]
public abstract class ConnectionParameters
{
}