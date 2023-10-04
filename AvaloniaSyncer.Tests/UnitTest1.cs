using System.Xml.Linq;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization.Model;
using CSharpFunctionalExtensions;
using Serilog;
using File = System.IO.File;

namespace AvaloniaSyncer.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var store = new ConfigurationStore(() => File.OpenRead("Connections.json"), () => File.OpenWrite("Connections.json"));
        var repoResult = await store.Load()
            .Map(list => new FileSystemConnectionRepository(list.Select(connection => Mapper.ToSystem(connection, Maybe<ILogger>.None))))
            .Check(repo =>
            {
                var connections = repo.Connections.Select(Mapper.ToConfiguration);
                return store.Save(new List<Connection>()
                {
                    new Connection()
                    {
                        Name = "Local system",
                        Parameters = new Local(),
                    },
                    new Connection()
                    {
                        Name = "PeachFS",
                        Parameters = new SeaweedFS()
                        {
                            Uri = new Uri("http://www.salute.com")
                        },
                    },
                    new Connection()
                    {
                        Name = "Raspberry Pi",
                        Parameters = new Sftp()
                        {
                            Host = "192.168.1.120",
                            Port = 22,
                            Username = "jmn",
                            Password = "password",
                        },
                    }

                });
            });
    }
}