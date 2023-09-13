namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization.Model;

public class Sftp : ConnectionParameters
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}