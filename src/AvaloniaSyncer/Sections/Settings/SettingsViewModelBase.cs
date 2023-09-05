using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaSyncer.Plugins;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Sections.Settings;

internal abstract class SettingsViewModelBase<TProfile, TDto> : ViewModelBase, IPluginSettings where TProfile : IProfile
{
    public SettingsViewModelBase(Maybe<ILogger> logger, string storeName, Func<TProfile> createProfile)
    {
        var store = new ObjectStore<IEnumerable<TDto>>(storeName);
        ProfilesManager = new ProfilesViewModel<TProfile>(
            logger,
            () => createProfile(),
            toSave => store.Save(toSave.Select(x => ToDto(x))),
            () => store.Load().Map(dtos => dtos.Select(FromDto)));
        Load = ProfilesManager.Load;
    }

    public ProfilesViewModel<TProfile> ProfilesManager { get; }
    public ICommand Load { get; }

    public async Task<Maybe<IEnumerable<IProfile>>> GetProfiles()
    {
        var profiles = await ProfilesManager.Load.Execute().Successes().FirstAsync();
        return Maybe<IEnumerable<IProfile>>.From(profiles.Cast<IProfile>());
    }

    protected abstract TDto ToDto(TProfile model);
    protected abstract TProfile FromDto(TDto dto);
}