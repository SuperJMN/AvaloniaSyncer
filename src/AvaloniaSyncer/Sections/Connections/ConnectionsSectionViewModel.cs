using System;
using System.Collections.ObjectModel;
using System.Reactive;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Connections;

public class ConnectionsSectionViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<IConfiguration> configurations;

    public ConnectionsSectionViewModel(IConnectionsRepository repo, INotificationService notificationService)
    {
        repo.Connections.AsObservableChangeSet()
            .Transform(Mapper.ToEditable)
            .Bind(out configurations)
            .Subscribe();

        AddOrUpdate = ReactiveCommand.Create(() =>
        {
            IFileSystemConnection connection = Mapper.ToConnection(CurrentConfiguration);
            repo.AddOrUpdate(connection);
        });
    }

    public ReactiveCommand<Unit, Unit> AddOrUpdate { get; }

    public ReadOnlyObservableCollection<IConfiguration> Configurations => configurations;

    [Reactive]
    public IConfiguration CurrentConfiguration { get; set; }

    public object Save { get; }
}