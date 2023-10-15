using System;
using System.Reactive.Linq;

namespace AvaloniaSyncer.Sections.Connections;

public class LocalConfigurationViewModel : IConfiguration
{
    public string Name { get; }
    public IObservable<bool> IsValid => Observable.Return(true);

    public LocalConfigurationViewModel(string name)
    {
        Name = name;
    }
}