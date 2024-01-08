using System;
using System.Collections.Generic;
using Zafiro.Avalonia.Controls.StringEditor;
using Zafiro.UI.Fields;

namespace AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;

public class SeaweedConfigurationViewModel : ConfigurationViewModelBase
{
    public SeaweedConfigurationViewModel(Guid id, string name, IConnectionsRepository connectionsRepository, Action<ConfigurationViewModelBase> onRemove) : base(id, name, connectionsRepository, onRemove)
    {
        AddressField = new StringField();
        AddressField.AddRule(s => !string.IsNullOrEmpty(s), "Cannot be empty");
        AddressField.AddRule(s => Uri.TryCreate(s, UriKind.Absolute, out _), "Invalid address");
    }
    
    public StringField AddressField { get; }
    protected override IEnumerable<IField> Fields => [Name, AddressField];
}