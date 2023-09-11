using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using AvaloniaSyncer.Plugins;
using AvaloniaSyncer.Sections.Settings;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AvaloniaSyncer.ViewModels;

public class PluginConfiguratorViewModel : ViewModelBase
{
    public PluginConfiguratorViewModel(IPlugin plugin)
    {
        Session = plugin.Create();
        Name = plugin.Name;
        Profiles = ReactiveCommand.CreateFromTask(() => plugin.Settings.Bind(x => x.GetProfiles())).Execute().FirstAsync();
        this.WhenAnyValue(x => x.SelectedProfile).WhereNotNull().Do(Session.SetProfile).Subscribe();
    }

    public IObservable<Maybe<IEnumerable<IProfile>>> Profiles { get; }

    public string Name { get; set; }

    public ISession Session { get; }

    [Reactive] public IProfile? SelectedProfile { get; set; }
}