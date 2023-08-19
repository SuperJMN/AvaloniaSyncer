using System;
using System.Reactive.Linq;
using DynamicData.Binding;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace AvaloniaSyncer.Plugins.SeaweedFS.Configuration;

public class ProfileViewModel : ReactiveValidationObject
{
    public ProfileViewModel(Guid id)
    {
        Id = id;
        this.ValidationRule(x => x.Name, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Address, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        IsDirty = this.WhenAnyPropertyChanged().Select(x => true).StartWith(false);
    }

    public IObservable<bool> IsDirty { get; set; }

    public Guid Id { get; private set; }

    [Reactive] public string Name { get; set; } = "";
    [Reactive] public string Address { get; set; } = "";
}