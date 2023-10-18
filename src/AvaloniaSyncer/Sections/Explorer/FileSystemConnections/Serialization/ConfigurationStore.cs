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
    private readonly Func<Result<Stream>> read;
    private readonly Func<Result<Stream>> write;

    public ConfigurationStore(Func<Result<Stream>> read, Func<Result<Stream>> write)
    {
        this.read = read;
        this.write = write;
    }

    public Task<Result<IEnumerable<Connection>>> Load()
    {
        return read()
            .Bind(stream =>
            {
                return Result.Try(async () =>
                {
                    using (stream)
                    {
                        return await JsonSerializer.DeserializeAsync<IEnumerable<Connection>>(stream);
                    }
                });
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
        return write()
            .Bind(stream =>
            {
                return Result.Try(async () =>
                {
                    using (stream)
                    {
                        await JsonSerializer.SerializeAsync(stream, connections);
                    }
                });
            });
    }
}