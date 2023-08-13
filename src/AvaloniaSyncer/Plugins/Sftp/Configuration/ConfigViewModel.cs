﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using Zafiro.Mixins;

namespace AvaloniaSyncer.Plugins.Sftp.Configuration;

public class ConfigViewModel : ViewModelBase
{
    private readonly SourceCache<ProfileViewModel, Guid> profilesSource;
    private readonly Repository repository = new();

    public ConfigViewModel()
    {
        profilesSource = new SourceCache<ProfileViewModel, Guid>(s => s.Id);
        CreateNewProfile = ReactiveCommand.Create(() => WorkingProfile = new ProfileViewModel(Guid.NewGuid()));

        Edit = ReactiveCommand.Create(() =>
        {
            WorkingProfile = new ProfileViewModel(SelectedProfile!.Id)
            {
                Host = SelectedProfile.Host,
                Name = SelectedProfile.Name,
                Username = SelectedProfile.Username,
                Password = SelectedProfile.Password,
                Port = SelectedProfile.Port
            };
        }, this.WhenAnyValue(x => x.SelectedProfile).WhereNotNull().SelectMany(x => x.IsValid()));

        AddOrUpdate = ReactiveCommand.Create(() =>
        {
            profilesSource!.AddOrUpdate(WorkingProfile);
            SelectedProfile = WorkingProfile;
        });

        profilesSource
            .Connect()
            .Sort(SortExpressionComparer<ProfileViewModel>.Ascending(s => s.Name))
            .Bind(out var collection)
            .Subscribe();

        Profiles = collection;

        Save = ReactiveCommand.CreateFromTask(OnSave);
        Load = ReactiveCommand.CreateFromTask(OnLoad);
        Delete = ReactiveCommand.Create(() => profilesSource.RemoveKey(SelectedProfile!.Id), this.WhenAnyValue(x => x.SelectedProfile).NotNull());
        AddOrUpdate.InvokeCommand(Save);
    }

    public ReactiveCommand<Unit, Unit> AddOrUpdate { get; set; }

    public ReactiveCommand<Unit, Unit> Delete { get; set; }

    public ReactiveCommand<Unit, Result> Load { get; }

    public ReactiveCommand<Unit, Result> Save { get; }

    public ReactiveCommand<Unit, Unit> Edit { get; set; }

    public ReactiveCommand<Unit, ProfileViewModel> CreateNewProfile { get; set; }

    [Reactive] public ProfileViewModel? SelectedProfile { get; set; }

    [Reactive] public ProfileViewModel? WorkingProfile { get; set; }

    public ReadOnlyObservableCollection<ProfileViewModel> Profiles { get; }

    private Task<Result> OnSave()
    {
        var toSave = new Config
        {
            Profiles = Profiles.Select(x => new ProfileDto
            {
                Id = x.Id,
                Name = x.Name,
                Host = x.Host,
                Port = x.Port,
                Username = x.Username,
                Password = x.Password
            }).ToList()
        };

        return repository.Save(toSave);
    }

    private async Task<Result> OnLoad()
    {
        var models = await repository.Load()
            .Map(dto =>
            {
                return dto.Profiles.Select(itemDto => new ProfileViewModel(itemDto.Id)
                {
                    Name = itemDto.Name,
                    Host = itemDto.Host,
                    Username = itemDto.Username,
                    Password = itemDto.Password,
                    Port = itemDto.Port
                });
            });

        return models.Tap(enumerable => profilesSource.Edit(x => x.Load(enumerable)));
    }
}