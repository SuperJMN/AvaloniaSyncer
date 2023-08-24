using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using Serilog;
using Zafiro.Mixins;

namespace AvaloniaSyncer.Plugins.SeaweedFS.Configuration;

public class SettingsViewModel : ViewModelBase, IPluginSettings
{
    private readonly Maybe<ILogger> logger;
    private readonly SourceCache<ProfileViewModel, Guid> profilesSource;
    private readonly Repository repository = new();

    public SettingsViewModel(Maybe<ILogger> logger)
    {
        this.logger = logger;
        profilesSource = new SourceCache<ProfileViewModel, Guid>(s => s.Id);
        CreateNewProfile = ReactiveCommand.Create(() => SelectedProfile = new ProfileViewModel(Guid.NewGuid()));

        Update = ReactiveCommand.Create(() => { profilesSource.AddOrUpdate(SelectedProfile!); }, this.WhenAnyValue(x => x.SelectedProfile).SelectMany(x => x is null ? Observable.Return(false) : x.IsValid()));

        profilesSource
            .Connect()
            .Sort(SortExpressionComparer<ProfileViewModel>.Ascending(s => s.Name))
            .Bind(out var collection)
            .Subscribe();

        Profiles = collection;

        Save = ReactiveCommand.CreateFromTask(OnSave);
        Load = ReactiveCommand.CreateFromTask(OnLoad);
        Delete = ReactiveCommand.Create(() => profilesSource.RemoveKey(SelectedProfile!.Id), this.WhenAnyValue(x => x.SelectedProfile).NotNull());

        Update.InvokeCommand(Save);
    }

    public ReactiveCommand<Unit, Unit> Delete { get; set; }

    public ReactiveCommand<Unit, Result> Save { get; }

    public ReactiveCommand<Unit, Unit> Update { get; set; }

    public ReactiveCommand<Unit, ProfileViewModel> CreateNewProfile { get; set; }

    [Reactive] public ProfileViewModel? SelectedProfile { get; set; }

    public ReadOnlyObservableCollection<ProfileViewModel> Profiles { get; }

    public ICommand Load { get; }

    private Task<Result> OnSave()
    {
        var toSave = new Config
        {
            Profiles = Profiles.Select(x => new ProfileDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address
            }).ToList()
        };

        return repository.Save(toSave).TapError(s => logger.Execute(l => l.Warning(s)));
    }

    private async Task<Result> OnLoad()
    {
        var models = await repository.Load()
            .Map(dto =>
            {
                return dto.Profiles.Select(itemDto => new ProfileViewModel(itemDto.Id)
                {
                    Name = itemDto.Name,
                    Address = itemDto.Address
                });
            });

        return models.Tap(enumerable => profilesSource.Edit(x => x.Load(enumerable)))
            .TapError(s => logger.Execute(l => l.Warning(s)));
    }
}