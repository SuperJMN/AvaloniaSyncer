﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
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
    private readonly Persistency persistency = new();
    private readonly SourceCache<SftpInfoViewModel, Guid> profilesSource;

    public ConfigViewModel()
    {
        profilesSource = new(s => s.Id);
        CreateNewProfile = ReactiveCommand.Create(() => WorkingProfile = new SftpInfoViewModel(Guid.NewGuid()));

        Edit = ReactiveCommand.Create(() =>
        {
            WorkingProfile = new SftpInfoViewModel(SelectedProfile.Id)
            {
                Host = SelectedProfile.Host,
                Name = SelectedProfile.Name,
                Username= SelectedProfile.Username,
                Password = SelectedProfile.Password,
                Port = SelectedProfile.Port,
            };
        }, this.WhenAnyValue(x => x.SelectedProfile).WhereNotNull().SelectMany(x => x.IsValid()));

        AddOrUpdate = ReactiveCommand.Create(() =>
        {
            profilesSource.AddOrUpdate(WorkingProfile);
            SelectedProfile = WorkingProfile;
        });

        profilesSource
            .Connect()
            .Sort(SortExpressionComparer<SftpInfoViewModel>.Ascending(s => s.Name))
            .Bind(out var collection)
            .Subscribe();

        Profiles = collection;

        Save = ReactiveCommand.CreateFromTask(OnSave);
        Load = ReactiveCommand.CreateFromTask(OnLoad);
        Delete = ReactiveCommand.Create(() => profilesSource.RemoveKey(SelectedProfile.Id));
        NewConfigAddedOrEdited = AddOrUpdate.Merge(CreateNewProfile.ToSignal());
        NewConfigAddedOrEdited.InvokeCommand(Save);
    }

    public ReactiveCommand<Unit, Unit> AddOrUpdate { get; set; }

    public ReactiveCommand<Unit, Unit> Delete { get; set; }

    public IObservable<Unit> NewConfigAddedOrEdited { get; set; }

    public ReactiveCommand<Unit, Unit> AddNew { get; set; }

    public ReactiveCommand<Unit, Result> Load { get; set; }

    public ReactiveCommand<Unit, Result> Save { get; set; }

    public ReactiveCommand<Unit, Unit> Edit { get; set; }

    public ReactiveCommand<Unit, SftpInfoViewModel> CreateNewProfile { get; set; }

    [Reactive] public SftpInfoViewModel SelectedProfile { get; set; } = new(Guid.NewGuid());
    [Reactive] public SftpInfoViewModel WorkingProfile { get; set; } = new(Guid.NewGuid());

    public ReadOnlyObservableCollection<SftpInfoViewModel> Profiles { get; }

    private Task<Result> OnSave()
    {
        var toSave = new ConfigDto()
        {
            Items = Profiles.Select(x => new ConfigItemDto()
            {
                Id = x.Id,
                Name = x.Name,
                Host = x.Host,
                Port = x.Port,
                Username = x.Username,
                Password = x.Password,
            }).ToList(),
        };

        return persistency.Save(toSave);
    }

    private async Task<Result> OnLoad()
    {
        var models = await persistency.Load()
            .Map(dto =>
            {
                return dto.Items.Select(itemDto => new SftpInfoViewModel(itemDto.Id)
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