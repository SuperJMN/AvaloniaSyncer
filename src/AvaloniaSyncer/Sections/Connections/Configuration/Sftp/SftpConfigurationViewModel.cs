using System;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Sftp;

public class SftpConfigurationViewModel : ConfigurationViewModelBase
{
    public SftpConfigurationViewModel(Guid id, string name, SftpConnectionParameters sftpConnectionParameters, IConnectionsRepository connectionsRepository) : base(id, name, connectionsRepository)
    {
        Host = sftpConnectionParameters.Host;
        Port = sftpConnectionParameters.Port;
        Username = sftpConnectionParameters.Username;
        Password = sftpConnectionParameters.Password;

        this.ValidationRule(x => x.Host, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Port, s => s is > 0 and < ushort.MaxValue, "Cannot be empty");
        this.ValidationRule(x => x.Username, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Password, s => !string.IsNullOrEmpty(s), "Cannot be empty");
    }

    [Reactive] public string Host { get; init; }
    [Reactive] public int Port { get; set; }
    [Reactive] public string Username { get; set; }
    [Reactive] public string Password { get; set; }
}