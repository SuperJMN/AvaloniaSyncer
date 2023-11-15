using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using AvaloniaSyncer.Sections.Connections.Configuration.Android;
using AvaloniaSyncer.Sections.Connections.Configuration.Local;
using AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;
using AvaloniaSyncer.Sections.Connections.Configuration.Sftp;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.Dialogs;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Connections;

public class ConnectionsSectionViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<IConfiguration> configurations;
    private readonly SourceCache<IConfiguration, Guid> configs = new(x => x.Id);

    public ConnectionsSectionViewModel(IConnectionsRepository repo, INotificationService notificationService, IDialogService dialogService)
    {
        configs.AddOrUpdate(repo.Connections.ToList().Select(connection => Mapper.ToConfiguration(connection, repo)));
        configs.Connect().Bind(out configurations).Subscribe();
        ReactiveCommand.Create(() =>
        {
            var connection = Mapper.ToConnection(CurrentConfiguration);
            repo.AddOrUpdate(connection);
        }, this.WhenAnyObservable(x => x.CurrentConfiguration.IsValid));

        Plugins = new IPlugin[]
            {
                OperatingSystem.IsAndroid() ? new AndroidPlugin() : new LocalPlugin(),
                new SeaweedFSPlugin(),
                new SftpPlugin(),
            }
            .Select(plugin => new PluginViewModel(plugin, configs, repo))
            .ToList();
    }

    public ReadOnlyObservableCollection<IConfiguration> Configurations => configurations;

    [Reactive]
    public IConfiguration? CurrentConfiguration { get; set; }

    public IEnumerable<PluginViewModel> Plugins { get; }
}