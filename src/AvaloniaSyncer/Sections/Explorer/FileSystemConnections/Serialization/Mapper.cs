using System;
using AvaloniaSyncer.Sections.Connections;
using AvaloniaSyncer.Sections.Connections.Configuration;
using AvaloniaSyncer.Sections.Connections.Configuration.Local;
using AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;
using AvaloniaSyncer.Sections.Connections.Configuration.Sftp;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization.Model;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;

public static class Mapper
{
    public static Connection ToConfiguration(IZafiroFileSystemConnection fileSystemConnection)
    {
        return fileSystemConnection switch
        {
            LocalFileSystemConnection local => new Connection
            {
                Id = local.Id,
                Name = local.Name,
                Parameters = new Local()
            },
            SeaweedFSFileSystemConnection seaweed => new Connection
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
            _ => throw new ArgumentOutOfRangeException(nameof(fileSystemConnection))
        };
    }

    public static IZafiroFileSystemConnection ToSystem(Connection connection, Maybe<ILogger> logger)
    {
        switch (connection.Parameters)
        {
            case Local:
                return new LocalFileSystemConnection(connection.Id, connection.Name);
            case SeaweedFS fs:
                return new SeaweedFSFileSystemConnection(connection.Id, connection.Name, fs.Uri, logger);
            case Sftp sftp:
                var info = new SftpConnectionParameters(sftp.Host, sftp.Port, sftp.Username, sftp.Password);
                return new SftpFileSystemConnection(connection.Id, connection.Name, info);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static IConfiguration ToConfiguration(IZafiroFileSystemConnection connection, IConnectionsRepository repo, Action<ConfigurationViewModelBase> onRemove)
    {
        return connection switch
        {
            LocalFileSystemConnection local => new LocalConfigurationViewModel(local.Id, local.Name, repo, onRemove),
            SeaweedFSFileSystemConnection seaweed => new SeaweedFSConfigurationViewModel(seaweed.Id, seaweed.Name, seaweed.Uri, repo, onRemove),
            SftpFileSystemConnection sftp => new SftpConfigurationViewModel(sftp.Id, sftp.Name, sftp.Parameters, repo, onRemove),
            _ => throw new ArgumentOutOfRangeException(nameof(connection))
        };
    }

    public static IZafiroFileSystemConnection ToConnection(IConfiguration currentConfiguration)
    {
        return currentConfiguration switch
        {
            SeaweedFSConfigurationViewModel swfs => new SeaweedFSFileSystemConnection(swfs.Id, swfs.Name.Value, new Uri(swfs.AddressField.Value), Maybe<ILogger>.None),
            LocalConfigurationViewModel local => new LocalFileSystemConnection(local.Id, local.Name.Value),
            SftpConfigurationViewModel sftp => new SftpFileSystemConnection(sftp.Id,
                sftp.Name.Value,
                new SftpConnectionParameters(sftp.HostField.Value,
                    sftp.Port, sftp.HostField.Value,
                    sftp.HostField.Value)),
            _ => throw new ArgumentOutOfRangeException(nameof(currentConfiguration))
        };
    }
}