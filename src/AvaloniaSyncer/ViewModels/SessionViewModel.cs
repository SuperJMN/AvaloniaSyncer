using System;
using System.Reactive.Linq;
using AvaloniaSyncer.Plugins;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.UI;

namespace AvaloniaSyncer.ViewModels;

public class SessionViewModel : ViewModelBase
{
    public SessionViewModel(string name, PluginSelection selection, INotificationService notificationService, Maybe<ILogger> logger)
    {
        Name = name;
        Selection = selection;
        SourceSession = selection.Source.Create();
        DestinationSession = selection.Destination.Create();

        Materialized = SourceSession.Directory
            .CombineLatest(DestinationSession.Directory, (source, dest) => Maybe.From(new SynchronizationViewModel("Título", notificationService, source, dest, logger)))
            .StartWith(Maybe<SynchronizationViewModel>.None);
    }

    public IObservable<Maybe<SynchronizationViewModel>> Materialized { get; }

    public ISession DestinationSession { get; }

    public ISession SourceSession { get; }

    public string Name { get; }

    public PluginSelection Selection { get; }
}