using System;
using System.Collections.Generic;
using Zafiro.UI.Fields;

namespace AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;

public class SeaweedFSConfigurationViewModel : ConfigurationViewModelBase
{
    public SeaweedFSConfigurationViewModel(Guid id, string name, Uri address, IConnectionsRepository connectionsRepository, Action<ConfigurationViewModelBase> onRemove) : base(id, name, connectionsRepository, onRemove)
    {
        AddressField = new Field<string>(address.ToString());
        AddressField.Validate(s => !string.IsNullOrEmpty(s), "Cannot be empty");
        AddressField.Validate(s => Uri.TryCreate(s, UriKind.Absolute, out _), "Invalid address");
    }
    
    public Field<string> AddressField { get; }
    protected override IEnumerable<IField> Fields => [Name, AddressField];
}