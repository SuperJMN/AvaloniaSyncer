using System;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace AvaloniaSyncer.Plugins.SeaweedFS_new.Configuration;

public class ConfigurationViewModel : ReactiveValidationObject, ISessionConfiguration
{
    public ConfigurationViewModel()
    {
        this.ValidationRule(x => x.Address, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Address, s => Uri.TryCreate(s, UriKind.Absolute, out _), "Invalid address");
    }

    [Reactive]
    public string Address { get; set; } = "";
}