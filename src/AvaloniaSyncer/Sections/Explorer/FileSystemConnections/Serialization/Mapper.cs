using System;
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
            SeaweedFileFileSystemConnection seaweedFileFileSystemConnection => new SeaweedConfiguration(seaweedFileFileSystemConnection.Name),
            LocalFileSystemConnection localFileSystemConnection => new LocalConfiguration(localFileSystemConnection.Name),
            SftpFileFileSystemConnection sftpFileFileSystemConnection => new SftpConfiguration(sftpFileFileSystemConnection.Name),
            _ => throw new ArgumentOutOfRangeException(nameof(connection))
        };
    }
}

public class SftpConfiguration : IConfiguration
{
    public string Name { get; }

    public SftpConfiguration(string name)
    {
        Name = name;
    }
}

public class LocalConfiguration : IConfiguration
{
    public string Name { get; }

    public LocalConfiguration(string name)
    {
        Name = name;
    }
}

public class SeaweedConfiguration : IConfiguration
{
    public string Name { get; }

    public SeaweedConfiguration(string name)
    {
        Name = name;
    }
}

public interface IConfiguration
{
    public string Name { get; }
}