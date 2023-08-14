using System.IO;
using System.IO.IsolatedStorage;
using System.Text.Json;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Plugins.SeaweedFS.Configuration;

public class Repository
{
    public Task<Result> Save(Config dto)
    {
        return OpenWrite("SeaweedFS").Bind(stream => Save(dto, stream));
    }

    private static Task<Result> Save(Config dto, Stream stream)
    {
        return Result.Try(async () =>
        {
            await using (stream)
            {
                await JsonSerializer.SerializeAsync(stream, dto);
            }
        });
    }

    private static Task<Result<Config>> Load(Stream stream)
    {
        return Result.Try(async () =>
        {
            await using (stream)
            {
                return await JsonSerializer.DeserializeAsync<Config>(stream);
            }
        }).EnsureNotNull("Could not load");
    }

    public Task<Result<Config>> Load()
    {
        return OpenRead("SeaweedFS").Bind(stream => Load(stream));
    }

    private static Result<Stream> OpenRead(string path)
    {
        var isolatedStorageScope = IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly;
        return Result.Try(() => IsolatedStorageFile.GetStore(isolatedStorageScope, null, null))
            .Map(file => (Stream)file.OpenFile(path, FileMode.Open));
    }

    private static Result<Stream> OpenWrite(string path)
    {
        var isolatedStorageScope = IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly;
        return Result.Try(() => IsolatedStorageFile.GetStore(isolatedStorageScope, null, null))
            .Map(file => (Stream)file.OpenFile(path, FileMode.Create));
    }

}