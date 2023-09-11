using System;
using System.Reactive.Linq;
using AvaloniaSyncer.Plugins.Sftp.Configuration;
using AvaloniaSyncer.Sections.Settings;
using DynamicData.Binding;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace AvaloniaSyncer.Plugins.Sftp.Settings;

public class Profile : ReactiveValidationObject, IProfile
{
    public Profile(Guid id)
    {
        Id = id;
        IsDirty = this.WhenAnyPropertyChanged().Select(x => true).StartWith(false);
        Configuration = new ConfigurationViewModel();
    }

    public ConfigurationViewModel Configuration { get; }
    public IObservable<bool> IsDirty { get; }
    public Guid Id { get; }
    public string Name { get; set; } = "";
    public IObservable<bool> IsValid => this.IsValid();
}