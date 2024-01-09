using System;
using System.Collections.Generic;
using Zafiro.Avalonia.Controls.StringEditor;
using Zafiro.UI.Fields;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Sftp;

public class SftpConfigurationViewModel : ConfigurationViewModelBase
{
    public SftpConfigurationViewModel(Guid id, string name, SftpConnectionParameters sftpConnectionParameters, IConnectionsRepository connectionsRepository, Action<ConfigurationViewModelBase> onRemove) : base(id, name, connectionsRepository, onRemove)
    {
        HostField = new StringField(sftpConnectionParameters.Host);
        PortField = new Field<int?>(sftpConnectionParameters.Port);
        UsernameField = new StringField(sftpConnectionParameters.Username);
        PasswordField = new StringField(sftpConnectionParameters.Password);

        HostField.Validate(s => !string.IsNullOrEmpty(s), "Cannot be empty");
        PortField.Validate(s => s is > 0 and < ushort.MaxValue, "Cannot be empty");
        UsernameField.Validate(s => !string.IsNullOrEmpty(s), "Cannot be empty");
        PasswordField.Validate(s => !string.IsNullOrEmpty(s), "Cannot be empty");
    }

    public Field<int?> PortField { get; }
    public StringField HostField { get; }
    public StringField UsernameField { get; }
    public StringField PasswordField { get; }

    protected override IEnumerable<IField> Fields => [Name, HostField, UsernameField, PortField, PasswordField];
}