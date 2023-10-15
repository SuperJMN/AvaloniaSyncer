using System;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;

public class SeaweedConfigurationViewModel : ReactiveValidationObject, IConfiguration
{
    public string Name { get; }
    public IObservable<bool> IsValid => this.IsValid();

    public SeaweedConfigurationViewModel(string name)
    {
        Name = name;
        this.ValidationRule(x => x.Address, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Address, s => Uri.TryCreate(s, UriKind.Absolute, out _), "Invalid address");
    }

    [Reactive]
    public string Address { get; set; } = "";
}