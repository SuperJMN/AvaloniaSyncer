using System.IO;
using System.IO.IsolatedStorage;
using System.Text.Json;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Plugins.Sftp.Configuration;

public class Persistency
{
    public Task<Result> Save(ConfigDto dto)
    {
        return OpenWrite("SFTP").Bind(stream => Save(dto, stream));
    }

    private static Task<Result> Save(ConfigDto dto, Stream stream)
    {
        return Result.Try(async () =>
        {
            await using (stream)
            {
                await JsonSerializer.SerializeAsync(stream, dto);
            }
        });
    }

    private static Task<Result<ConfigDto>> Load(Stream stream)
    {
        return Result.Try(async () =>
        {
            await using (stream)
            {
                return await JsonSerializer.DeserializeAsync<ConfigDto>(stream);
            }
        }).EnsureNotNull("Could not load");
    }

    public Task<Result<ConfigDto>> Load()
    {
        return OpenRead("SFTP").Bind(stream => Load(stream));
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