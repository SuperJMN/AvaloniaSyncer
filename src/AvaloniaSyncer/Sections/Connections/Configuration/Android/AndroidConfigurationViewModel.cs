using System;
using System.Reactive.Linq;
using AvaloniaSyncer.Controls;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Android;

public class AndroidConfigurationViewModel : IConfiguration
{
    public IObservable<bool> IsValid => Observable.Return(true);
    public StringProperty Name { get; }
    public Guid Id { get; set; }

    public AndroidConfigurationViewModel(Guid id, IConnectionsRepository connectionsRepository)
    {
        Id = id;
        Name = new StringProperty("Saludos");
    }
}