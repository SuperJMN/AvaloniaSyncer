using System;
using AvaloniaSyncer.Settings;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.Sftp.Settings;

internal class SettingsViewModel : SettingsViewModelBase<Profile, ProfileDto>
{
    public SettingsViewModel(Maybe<ILogger> logger) : base(logger, "Sftp.Profiles", () => new Profile(Guid.NewGuid()))
    {
    }

    protected override ProfileDto ToDto(Profile model)
    {
        return new ProfileDto
        {
            Name = model.Name,
            Id = model.Id,
            Host = model.Configuration.Host,
            Port = model.Configuration.Port,
            Username = model.Configuration.Username,
            Password = model.Configuration.Password,
        };
    }

    protected override Profile FromDto(ProfileDto dto)
    {
        return new Profile(dto.Id)
        {
            Name = dto.Name,
            Configuration =
            {
                Host = dto.Host,
                Port = dto.Port,
                Username = dto.Username,
                Password = dto.Password,
            }
        };
    }
}