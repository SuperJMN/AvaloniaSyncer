using System;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;

namespace AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;

public class SeaweedConfigurationViewModel : ConfigurationViewModelBase
{
    public SeaweedConfigurationViewModel(Guid id, string name, IConnectionsRepository connectionsRepository) : base(id, name, connectionsRepository)
    {
        this.ValidationRule(x => x.Address, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Address, s => Uri.TryCreate(s, UriKind.Absolute, out _), "Invalid address");
    }

    [Reactive] public string Address { get; init; } = "";
}