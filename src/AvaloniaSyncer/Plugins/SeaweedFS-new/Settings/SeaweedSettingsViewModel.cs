using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using AvaloniaSyncer.Settings;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.SeaweedFS_new.Settings;

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

    private SeaweedProfileDto ToDto(SeaweedProfile model)
    {
        return new SeaweedProfileDto
        {
            Name = model.Name,
            Address = model.Address,
            Id = model.Id
        };
    }

    private SeaweedProfile FromDto(SeaweedProfileDto dto)
    {
        return new SeaweedProfile(dto.Id)
        {
            Name = dto.Name,
            Address = dto.Address
        };
    }
}