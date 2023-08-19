﻿using System;
using System.Net;
using System.Reactive.Linq;
using DynamicData.Binding;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace AvaloniaSyncer.Plugins.Sftp.Configuration;

public class ProfileViewModel : ReactiveValidationObject
{
    public ProfileViewModel(Guid id)
    {
        Id = id;
        this.ValidationRule(x => x.Name, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Host, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Port, s => s is > 0 and < ushort.MaxValue, "Cannot be empty");
        this.ValidationRule(x => x.Username, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        this.ValidationRule(x => x.Password, s => !string.IsNullOrEmpty(s), "Cannot be empty");
        IsDirty = this.WhenAnyPropertyChanged().Select(x => true).StartWith(false);
    }

    public IObservable<bool> IsDirty { get; set; }

    public Guid Id { get; private set; }

    [Reactive] public string Name { get; set; } = "";
    [Reactive] public string Host { get; set; } = "";
    [Reactive] public int Port { get; set; } = 22;
    [Reactive] public string Username { get; set; } = "";
    [Reactive] public string Password { get; set; } = "";
}