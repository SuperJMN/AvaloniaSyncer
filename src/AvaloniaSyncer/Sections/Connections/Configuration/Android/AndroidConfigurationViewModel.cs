using System;
using System.Reactive.Linq;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Android;

public class AndroidConfigurationViewModel : IConfiguration
{
    public string Name { get; }
    public IObservable<bool> IsValid => Observable.Return(true);

    public AndroidConfigurationViewModel(string name)
    {
        Name = name;
    }
}