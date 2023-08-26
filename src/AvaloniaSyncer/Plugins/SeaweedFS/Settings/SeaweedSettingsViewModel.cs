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

namespace AvaloniaSyncer.Plugins.SeaweedFS.Settings;

internal class SeaweedSettingsViewModel : ViewModelBase, IPluginSettings
{
    public SeaweedSettingsViewModel(Maybe<ILogger> logger)
    {
        var store = new ObjectStore<IEnumerable<SeaweedProfileDto>>("SeaweedFS.Profiles");
        ProfilesManager = new ProfilesViewModel<SeaweedProfile>(
            logger,
            () => new SeaweedProfile(Guid.NewGuid()),
            toSave => store.Save(toSave.Select(ToDto)),
            () => store.Load().Map(dtos => dtos.Select(FromDto)));
        Load = ProfilesManager.Load;
    }

    public ProfilesViewModel<SeaweedProfile> ProfilesManager { get; }

    public ICommand Load { get; }
    public async Task<Maybe<IEnumerable<IProfile>>> GetProfiles()
    {
        var seaweedProfiles = await ProfilesManager.Load.Execute().Successes().FirstAsync();
        return Maybe<IEnumerable<IProfile>>.From(seaweedProfiles);
    }

    private SeaweedProfileDto ToDto(SeaweedProfile model)
    {
        return new SeaweedProfileDto
        {
            Name = model.Name,
            Address = model.Configuration.Address,
            Id = model.Id
        };
    }

    private SeaweedProfile FromDto(SeaweedProfileDto dto)
    {
        return new SeaweedProfile(dto.Id)
        {
            Name = dto.Name,
            Configuration =
            {
                Address = dto.Address,
            }
        };
    }
}