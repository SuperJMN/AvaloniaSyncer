using System;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace AvaloniaSyncer.Sections.Connections.Configuration.Sftp;

public class SftpConfigurationViewModel : ReactiveValidationObject, IConfiguration
{
    public SftpConfigurationViewModel(string name)
    {
        Name = name;
        this.ValidationRule(x => x.Host, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Port, s => s is > 0 and < ushort.MaxValue, "Cannot be empty");
        this.ValidationRule(x => x.Username, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Password, s => !string.IsNullOrEmpty(s), "Cannot be empty"); ;
    }

    public string Name { get; }
    public IObservable<bool> IsValid => this.IsValid();
    [Reactive] public string Host { get; set; } = "";
    [Reactive] public int Port { get; set; } = 22;
    [Reactive] public string Username { get; set; } = "";
    [Reactive] public string Password { get; set; } = "";
}