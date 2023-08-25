using System;
using System.Reactive.Linq;
using AvaloniaSyncer.Settings;
using DynamicData.Binding;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace AvaloniaSyncer.Plugins.SeaweedFS_new.Settings;

public class SeaweedProfile : ReactiveValidationObject, IProfile
{
    public SeaweedProfile(Guid id)
    {
        Id = id;
        IsDirty = this.WhenAnyPropertyChanged().Select(x => true).StartWith(false);
    }

    public string Address { get; set; } = "";
    public IObservable<bool> IsDirty { get; }
    public Guid Id { get; }
    public string Name { get; set; } = "";
    public IObservable<bool> IsValid => this.IsValid();
}