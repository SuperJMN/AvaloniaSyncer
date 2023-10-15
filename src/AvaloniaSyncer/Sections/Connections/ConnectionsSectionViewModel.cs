using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer.FileSystemConnections.Serialization;
using CSharpFunctionalExtensions;
using DynamicData;
using Microsoft.CodeAnalysis.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.Dialogs;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Mixins;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Connections;

public class ConnectionsSectionViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<IConfiguration> configurations;
    private readonly SourceCache<IConfiguration, string> configs = new(x => x.Name);

    public ConnectionsSectionViewModel(IConnectionsRepository repo, INotificationService notificationService, IDialogService dialogService)
    {
        configs.AddOrUpdate(repo.Connections.Select(Mapper.ToEditable));

        configs.Connect().Bind(out configurations).Subscribe();

        AddOrUpdate = ReactiveCommand.Create(() =>
        {
            var connection = Mapper.ToConnection(CurrentConfiguration);
            repo.AddOrUpdate(connection);
        }, this.WhenAnyObservable(x => x.CurrentConfiguration.IsValid));

        AddNew = ReactiveCommand.CreateFromObservable(() =>
        {
            return Observable.FromAsync(() => dialogService.ShowDialog<CreateNewConnectionDialogViewModel, IConfiguration>(new CreateNewConnectionDialogViewModel(configurations.Select(x => x.Name)), "Create new connection"))
                .Values()
                .Select(configuration =>
                {
                    configs.AddOrUpdate(configuration);
                    return Unit.Default;
                });
        });
    }

    public ReactiveCommand<Unit,Unit> AddNew { get; set; }

    public ReactiveCommand<Unit, Unit> AddOrUpdate { get; }

    public ReadOnlyObservableCollection<IConfiguration> Configurations => configurations;

    [Reactive]
    public IConfiguration CurrentConfiguration { get; set; }
}