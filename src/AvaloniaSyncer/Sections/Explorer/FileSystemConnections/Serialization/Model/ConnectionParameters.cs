using System.Text.Json.Serialization;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization.Model;

[JsonDerivedType(typeof(Local), nameof(Local))]
[JsonDerivedType(typeof(SeaweedFS), nameof(SeaweedFS))]
[JsonDerivedType(typeof(Sftp), nameof(Sftp))]
[JsonDerivedType(typeof(Android), nameof(Android))]
public abstract class ConnectionParameters
{
}