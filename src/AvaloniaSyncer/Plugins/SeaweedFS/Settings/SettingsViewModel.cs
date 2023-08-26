using System;
using AvaloniaSyncer.Settings;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.SeaweedFS.Settings;

internal class SettingsViewModel : SettingsViewModelBase<Profile, ProfileDto>
{
    public SettingsViewModel(Maybe<ILogger> logger) : base(logger, "SeaweedFS.Profiles", () => new Profile(Guid.NewGuid()))
    {
    }

    protected override ProfileDto ToDto(Profile model)
    {
        return new ProfileDto
        {
            Name = model.Name,
            Address = model.Configuration.Address,
            Id = model.Id
        };
    }

    protected override Profile FromDto(ProfileDto dto)
    {
        return new Profile(dto.Id)
        {
            Name = dto.Name,
            Configuration =
            {
                Address = dto.Address,
            }
        };
    }
}