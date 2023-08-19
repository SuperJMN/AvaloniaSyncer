using System.IO;
using System.IO.IsolatedStorage;
using System.Text.Json;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Plugins.Sftp.Configuration;

public class Repository
{
    public Task<Result> Save(Config dto)
    {
        return OpenWrite("SFTP").Bind(stream => Save(dto, stream));
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
        return OpenRead("SFTP").Bind(stream => Load(stream));
    }

    private static Result<Stream> OpenRead(string path)
    {
        return Result.Try(GetStore)
            .Bind(store =>
            {
                return Result.Try(() =>
                {
                    var isolatedStorageFileStream = new IsolatedStorageFileStream(path, FileMode.Open, store);
                    return (Stream) isolatedStorageFileStream;
                });
            });
    }

    private static Result<Stream> OpenWrite(string path)
    {
        return Result.Try(GetStore)
            .Map(store => (Stream)new IsolatedStorageFileStream(path, FileMode.Create, store));
    }

    private static IsolatedStorageFile GetStore()
    {
        var isolatedStorageFile = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
        return isolatedStorageFile;
    }
}