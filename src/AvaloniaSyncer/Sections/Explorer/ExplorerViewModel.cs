using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.Explorer;

public class ExplorerViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<ReadOnlyObservableCollection<IFileSystemConnection>> connections;

    public ExplorerViewModel()
    {
        Load = ReactiveCommand.CreateFromTask(LoadFromTask);
        connections = Load.Successes().Select(x => x.Connections).ToProperty(this, x => x.Connections);
    }

    public ReactiveCommand<Unit, Result<FileSystemConnectionRepository>> Load { get; }

    public ReadOnlyObservableCollection<IFileSystemConnection> Connections => connections.Value;

    private async Task<Result<FileSystemConnectionRepository>> LoadFromTask()
    {
        var store = new ConfigurationStore(() => File.OpenRead("Connections.json"), () => File.OpenWrite("Connections.json"));
        var configs = await Async.Await(() => store.Load())
            .Map(enumerable => enumerable.Select(connection => Mapper.ToSystem(connection)))
            .Map(enumerable => new FileSystemConnectionRepository(enumerable));

        return configs;
    }
}