using System;
using System.Collections.Generic;
using Zafiro.UI.Fields;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Local;

public class LocalConfigurationViewModel : ConfigurationViewModelBase
{
    public LocalConfigurationViewModel(Guid id, string name, IConnectionsRepository connectionsRepository, Action<ConfigurationViewModelBase> onRemove) : base(id, name, connectionsRepository, onRemove)
    {
    }

    protected override IEnumerable<IField> Fields => [Name];
}