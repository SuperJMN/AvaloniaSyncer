using System;
using AvaloniaSyncer.Controls;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Sftp;

public class SftpConfigurationViewModel : ReactiveValidationObject, IConfiguration
{
    public SftpConfigurationViewModel(Guid id)
    {
        Id = id;
        this.ValidationRule(x => x.Host, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Port, s => s is > 0 and < ushort.MaxValue, "Cannot be empty");
        this.ValidationRule(x => x.Username, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Password, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        Name = new StringProperty("Saludos");
    }

    [Reactive] public string Host { get; set; } = "";
    [Reactive] public int Port { get; set; } = 22;
    [Reactive] public string Username { get; set; } = "";
    [Reactive] public string Password { get; set; } = "";
    public Guid Id { get; set; }

    public IObservable<bool> IsValid => this.IsValid();
    public StringProperty Name { get; }
}