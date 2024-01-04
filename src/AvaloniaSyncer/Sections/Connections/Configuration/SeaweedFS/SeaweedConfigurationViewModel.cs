using System;
using System.Linq;
using System.Reactive.Linq;
using Zafiro.Avalonia.Controls.StringEditor;
using Zafiro.UI.Fields;

namespace AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;

public class SeaweedConfigurationViewModel : ConfigurationViewModelBase
{
    public SeaweedConfigurationViewModel(Guid id, string name, IConnectionsRepository connectionsRepository) : base(id, name, connectionsRepository)
    {
        AddressField = new StringField();
        AddressField.AddRule(s => !string.IsNullOrEmpty(s), "Cannot be empty");
        AddressField.AddRule(s => Uri.TryCreate(s, UriKind.Absolute, out _), "Invalid address");
        Save.InvokeCommand(AddressField.Commit);
    }

    public StringField AddressField { get; }
    public override IObservable<bool> IsValid => Observable.CombineLatest([Name.IsValid, AddressField.IsValid]).Select(list => list.All(valid => valid));
    public override IObservable<bool> IsDirty => Observable.CombineLatest([Name.IsDirty, AddressField.IsDirty]).Select(list => list.Any(dirty => dirty));
}