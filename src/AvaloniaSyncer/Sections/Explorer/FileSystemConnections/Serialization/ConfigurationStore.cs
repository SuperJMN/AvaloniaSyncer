using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization.Model;
using CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;

public class ConfigurationStore
{
    private readonly Func<Stream> read;
    private readonly Func<Stream> write;

    public ConfigurationStore(Func<Stream> read, Func<Stream> write)
    {
        this.read = read;
        this.write = write;
    }

    public Task<Result<IEnumerable<Connection>>> Load()
    {
        return Result
            .Try(async () =>
            {
                using (var utf8Json = read())
                {
                    return await JsonSerializer.DeserializeAsync<IEnumerable<Connection>>(utf8Json);
                }
            })
            .OnFailureCompensate(() => Task.FromResult(Result.Success(Enumerable.Empty<Connection>())))
            .Bind(list =>
            {
                if (list == null)
                {
                    return Result.Success(Enumerable.Empty<Connection>());
                }

                return Result.Success(list);
            });
    }

    public Task<Result> Save(IEnumerable<Connection> connections)
    {
        return Result.Try(async () =>
        {
            await using (var utf8Json = write())
            {
                await JsonSerializer.SerializeAsync(utf8Json, connections);
            }
        });
    }
}