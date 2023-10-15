using System;
using AvaloniaSyncer.Sections.Connections;
using AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;
using AvaloniaSyncer.Sections.Connections.Configuration.Sftp;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization.Model;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;

public static class Mapper
{
    public static Connection ToConfiguration(IFileSystemConnection fileSystemConnection)
    {
        return fileSystemConnection switch
        {
            LocalFileSystemConnection local => new Connection
            {
                Name = local.Name,
                Parameters = new Local()
            },
            SeaweedFileFileSystemConnection seaweed => new Connection
            {
                Name = seaweed.Name,
                Parameters = new SeaweedFS
                {
                    Uri = seaweed.Uri
                }
            },
            SftpFileFileSystemConnection sftp =>
                new Connection
                {
                    Name = sftp.Name,
                    Parameters = new Sftp
                    {
                        Host = sftp.Parameters.Host,
                        Port = sftp.Parameters.Port,
                        Username = sftp.Parameters.Username,
                        Password = sftp.Parameters.Password
                    }
                },
            _ => throw new ArgumentOutOfRangeException(nameof(fileSystemConnection))
        };
    }

    public static IFileSystemConnection ToSystem(Connection connection, Maybe<ILogger> logger)
    {
        switch (connection.Parameters)
        {
            case Local:
                return new LocalFileSystemConnection(connection.Name);
            case SeaweedFS fs:
                return new SeaweedFileFileSystemConnection(connection.Name, fs.Uri, logger);
            case Sftp sftp:
                var info = new SftpConnectionParameters(sftp.Host, sftp.Port, sftp.Username, sftp.Password);
                return new SftpFileFileSystemConnection(connection.Name, info);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static IConfiguration ToEditable(IFileSystemConnection connection)
    {
        return connection switch
        {
            SeaweedFileFileSystemConnection seaweedFileFileSystemConnection => new SeaweedConfigurationViewModel(seaweedFileFileSystemConnection.Name)
            {
                Address = seaweedFileFileSystemConnection.Uri.ToString()
            },
            LocalFileSystemConnection localFileSystemConnection => new LocalConfigurationViewModel(localFileSystemConnection.Name),
            SftpFileFileSystemConnection sftp => new SftpConfigurationViewModel(sftp.Name)
            {
                Host = sftp.Parameters.Host,
                Port = sftp.Parameters.Port,
                Username = sftp.Parameters.Username,
                Password = sftp.Parameters.Password,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(connection))
        };
    }

    public static IFileSystemConnection ToConnection(IConfiguration currentConfiguration)
    {
        return currentConfiguration switch
        {
            SeaweedConfigurationViewModel swfs => new SeaweedFileFileSystemConnection(swfs.Name, new Uri(swfs.Address), Maybe<ILogger>.None),
            LocalConfigurationViewModel local => throw new ArgumentOutOfRangeException(nameof(currentConfiguration)),
            SftpConfigurationViewModel sftp => new SftpFileFileSystemConnection(
                sftp.Name,
                new SftpConnectionParameters(sftp.Host,
                    sftp.Port, sftp.Username,
                    sftp.Password)),
            _ => throw new ArgumentOutOfRangeException(nameof(currentConfiguration))
        };
    }
}