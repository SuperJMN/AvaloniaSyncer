using System;
using System.Collections.Generic;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using Zafiro.Avalonia.Controls.StringEditor;
using Zafiro.UI.Fields;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Sftp;

public class SftpConfigurationViewModel : ConfigurationViewModelBase
{
    public SftpConfigurationViewModel(Guid id, string name, SftpConnectionParameters sftpConnectionParameters, IConnectionsRepository connectionsRepository, Action<ConfigurationViewModelBase> onRemove) : base(id, name, connectionsRepository, onRemove)
    {
        HostField = new StringField(sftpConnectionParameters.Host);
        Port = sftpConnectionParameters.Port;
        UsernameField = new StringField(sftpConnectionParameters.Username);
        PasswordField = new StringField(sftpConnectionParameters.Password);

        HostField.AddRule(s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Port, s => s is > 0 and < ushort.MaxValue, "Cannot be empty");
        UsernameField.AddRule(s => !string.IsNullOrEmpty(s), "Cannot be empty");
        PasswordField.AddRule(s => !string.IsNullOrEmpty(s), "Cannot be empty");
    }

    public StringField HostField { get; }

    [Reactive] public int Port { get; set; }
    public StringField UsernameField { get; }
    public StringField PasswordField { get; }

    protected override IEnumerable<IField> Fields => [Name, HostField, UsernameField, PasswordField];
}