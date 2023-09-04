using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.UI;

namespace AvaloniaSyncer.ViewModels;

public class SessionViewModel : ViewModelBase
{
    public SessionViewModel(string name, PluginSelection selection, INotificationService notificationService, Maybe<ILogger> logger)
    {
        Name = name;
        SourceSession = new PluginConfiguratorViewModel(selection.Source);
        DestinationSession = new PluginConfiguratorViewModel(selection.Destination);

        Materialized = SourceSession.Session.Directory
            .CombineLatest(DestinationSession.Session.Directory, (source, dest) => Maybe.From(new SynchronizationViewModel(name, notificationService, source, dest, logger)))
            .StartWith(Maybe<SynchronizationViewModel>.None);
    }

    public IObservable<Maybe<SynchronizationViewModel>> Materialized { get; }

    public PluginConfiguratorViewModel DestinationSession { get; }

    public PluginConfiguratorViewModel SourceSession { get; }

    public string Name { get; }

    public PluginSelection Selection { get; }
}