using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.ViewModels;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Mixins;

namespace AvaloniaSyncer.Settings;

public class ProfilesViewModel<T> : ViewModelBase where T : IProfile
{
    private readonly Maybe<ILogger> logger;

    public ProfilesViewModel(Maybe<ILogger> logger, Func<T> create, Func<IEnumerable<T>, Task<Result>> onSave, Func<Task<Result<IEnumerable<T>>>> onLoad)
    {
        this.logger = logger;
        SourceCache<T, Guid> profilesSource = new(s => s.Id);
        CreateNewProfile = ReactiveCommand.Create(() => SelectedProfile = create());

        Update = ReactiveCommand.Create(() => { profilesSource.AddOrUpdate(SelectedProfile!); }, this.WhenAnyValue(x => x.SelectedProfile).SelectMany(x => x is null ? Observable.Return(false) : x.IsValid));

        profilesSource
            .Connect()
            .Sort(SortExpressionComparer<T>.Ascending(s => s.Name))
            .Bind(out var collection)
            .Subscribe();

        Profiles = collection;

        Save = ReactiveCommand.CreateFromTask(() => onSave(Profiles));
        Load = ReactiveCommand.CreateFromTask(onLoad);
        Delete = ReactiveCommand.Create(() => profilesSource.RemoveKey(SelectedProfile!.Id), this.WhenAnyValue(x => x.SelectedProfile).NotNull());

        Load.Successes()
            .Do(profiles => profilesSource.Edit(x => x.Load(profiles)))
            .Subscribe();

        Update.InvokeCommand(Save);
    }

    public ReactiveCommand<Unit, Result<IEnumerable<T>>> Load { get; set; }
    
    public ReactiveCommand<Unit, Unit> Delete { get; set; }

    public ReactiveCommand<Unit, Result> Save { get; }

    public ReactiveCommand<Unit, Unit> Update { get; set; }

    public ReactiveCommand<Unit, T> CreateNewProfile { get; set; }

    [Reactive] public T? SelectedProfile { get; set; }

    public ReadOnlyObservableCollection<T> Profiles { get; }

}