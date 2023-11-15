using System;
using AvaloniaSyncer.Sections.Connections;
using AvaloniaSyncer.Sections.Connections.Configuration.Android;
using AvaloniaSyncer.Sections.Connections.Configuration.Local;
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
                Id = local.Id,
                Name = local.Name,
                Parameters = new Local()
            },
            SeaweedFileSystemConnection seaweed => new Connection
            {
                Id = seaweed.Id,
                Name = seaweed.Name,
                Parameters = new SeaweedFS
                {
                    Uri = seaweed.Uri
                }
            },
            SftpFileSystemConnection sftp =>
                new Connection
                {
                    Id = sftp.Id,
                    Name = sftp.Name,
                    Parameters = new Sftp
                    {
                        Host = sftp.Parameters.Host,
                        Port = sftp.Parameters.Port,
                        Username = sftp.Parameters.Username,
                        Password = sftp.Parameters.Password
                    }
                },
            AndroidFileSystemConnection android =>
                new Connection()
                {
                    Id = android.Id,
                    Name = android.Name,
                    Parameters = new Android(),
                },
            _ => throw new ArgumentOutOfRangeException(nameof(fileSystemConnection))
        };
    }

    public static IFileSystemConnection ToSystem(Connection connection, Maybe<ILogger> logger)
    {
        switch (connection.Parameters)
        {
            case Local:
                return new LocalFileSystemConnection(connection.Id, connection.Name);
            case SeaweedFS fs:
                return new SeaweedFileSystemConnection(connection.Id, connection.Name, fs.Uri, logger);
            case Sftp sftp:
                var info = new SftpConnectionParameters(sftp.Host, sftp.Port, sftp.Username, sftp.Password);
                return new SftpFileSystemConnection(connection.Id, connection.Name, info);
            case Android:
                return new AndroidFileSystemConnection(connection.Id, connection.Name);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static IConfiguration ToConfiguration(IFileSystemConnection connection, IConnectionsRepository repo)
    {
        return connection switch
        {
            LocalFileSystemConnection local => new LocalConfigurationViewModel(local.Id, local.Name, repo),
            SeaweedFileSystemConnection seaweed => new SeaweedConfigurationViewModel(seaweed.Id, seaweed.Name, repo)
            {
                Address = seaweed.Uri.ToString(),
            },
            AndroidFileSystemConnection android => new AndroidConfigurationViewModel(android.Id, android.Name, repo),
            SftpFileSystemConnection sftp => new SftpConfigurationViewModel(sftp.Id, sftp.Name, repo)
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
            SeaweedConfigurationViewModel swfs => new SeaweedFileSystemConnection(swfs.Id, swfs.Name.Value, new Uri(swfs.Address), Maybe<ILogger>.None),
            LocalConfigurationViewModel local => new LocalFileSystemConnection(local.Id, local.Name.Value),
            SftpConfigurationViewModel sftp => new SftpFileSystemConnection(sftp.Id,
                sftp.Name.Value,
                new SftpConnectionParameters(sftp.Host,
                    sftp.Port, sftp.Username,
                    sftp.Password)),
            AndroidConfigurationViewModel android => new AndroidFileSystemConnection(android.Id, android.Name.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(currentConfiguration))
        };
    }
}