using System;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization.Model;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;

public static class Mapper
{
    public static Connection ToConfiguration(IFileSystemConnection fileSystemConnection)
    {
        return fileSystemConnection switch
        {
            LocalFileSystemConnection local => new Connection()
            {
                Name = local.Name,
                Parameters = new Local(),
            },
            SeaweedFileFileSystemConnection seaweed => new Connection()
            {
                Name = seaweed.Name,
                Parameters = new Local(),
            },
            SftpFileFileSystemConnection sftp =>
                new Connection()
                {
                    Name = sftp.Name,
                    Parameters = new Sftp()
                    {
                        Host = sftp.Parameters.Host,
                        Port = sftp.Parameters.Port,
                        Username = sftp.Parameters.Username,
                        Password = sftp.Parameters.Password,
                    },
                },
            _ => throw new ArgumentOutOfRangeException(nameof(fileSystemConnection))
        };
    }

    public static IFileSystemConnection ToSystem(Connection connection)
    {
        switch (connection.Parameters)
        {
            case Local:
                return new LocalFileSystemConnection(connection.Name);
            case SeaweedFS fs:
                return new SeaweedFileFileSystemConnection(connection.Name, fs.Uri);
            case Sftp sftp:
                var info = new SftpConnectionParameters(sftp.Host, sftp.Port, sftp.Username, sftp.Password);
                return new SftpFileFileSystemConnection(connection.Name, info);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}