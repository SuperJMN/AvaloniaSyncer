using System;
using AvaloniaSyncer.Settings;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace AvaloniaSyncer.Plugins.SeaweedFS_new.Settings;

public class SeaweedProfile : ReactiveValidationObject, IProfile
{
    public Guid Id { get; }

    public SeaweedProfile(Guid id)
    {
        Id = id;
    }

    public string Name { get; set; }
    public IObservable<bool> IsValid => this.IsValid();
}