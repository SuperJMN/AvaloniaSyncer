using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaSyncer.Settings;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Plugins.Sftp.Settings;

internal class SettingsViewModel : ViewModelBase, IPluginSettings
{
    public SettingsViewModel(Maybe<ILogger> logger)
    {
        var store = new ObjectStore<IEnumerable<ProfileDto>>("Sftp.Profiles");
        ProfilesManager = new ProfilesViewModel<Profile>(
            logger,
            () => new Profile(Guid.NewGuid()),
            toSave => store.Save(toSave.Select(ToDto)),
            () => store.Load().Map(dtos => dtos.Select(FromDto)));
        Load = ProfilesManager.Load;
    }

    public ProfilesViewModel<Profile> ProfilesManager { get; }

    public ICommand Load { get; }
    public async Task<Maybe<IEnumerable<IProfile>>> GetProfiles()
    {
        var seaweedProfiles = await ProfilesManager.Load.Execute().Successes().FirstAsync();
        return Maybe<IEnumerable<IProfile>>.From(seaweedProfiles);
    }

    private ProfileDto ToDto(Profile model)
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

    private Profile FromDto(ProfileDto dto)
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