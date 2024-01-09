using System;
using System.Collections.Generic;
using Zafiro.UI.Fields;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Sftp;

public class SftpConfigurationViewModel : ConfigurationViewModelBase
{
    public SftpConfigurationViewModel(Guid id, string name, SftpConnectionParameters sftpConnectionParameters, IConnectionsRepository connectionsRepository, Action<ConfigurationViewModelBase> onRemove) : base(id, name, connectionsRepository, onRemove)
    {
        HostField = new Field<string>(sftpConnectionParameters.Host);
        PortField = new Field<int?>(sftpConnectionParameters.Port);
        UsernameField = new Field<string>(sftpConnectionParameters.Username);
        PasswordField = new Field<string>(sftpConnectionParameters.Password);

        HostField.Validate(s => !string.IsNullOrEmpty(s), "Cannot be empty");
        PortField.Validate(s => s is > 0 and < ushort.MaxValue, "Cannot be empty");
        UsernameField.Validate(s => !string.IsNullOrEmpty(s), "Cannot be empty");
        PasswordField.Validate(s => !string.IsNullOrEmpty(s), "Cannot be empty");
    }

    public Field<int?> PortField { get; }
    public Field<string> HostField { get; }
    public Field<string> UsernameField { get; }
    public Field<string> PasswordField { get; }

    protected override IEnumerable<IField> Fields => [Name, HostField, UsernameField, PortField, PasswordField];
}