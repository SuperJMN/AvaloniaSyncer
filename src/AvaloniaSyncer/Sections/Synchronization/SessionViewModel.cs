using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using Zafiro.Avalonia.WizardOld.Interfaces;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Synchronizer;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Synchronization;

public class SessionViewModel : ReactiveValidationObject, IValidatable
{
    private readonly IZafiroDirectory destination;
    private readonly IZafiroDirectory source;

    public SessionViewModel(INotificationService notificationService, IZafiroDirectory source, IZafiroDirectory destination, Maybe<ILogger> logger)
    {
        this.source = source;
        this.destination = destination;
        SyncAll = StoppableCommand.Create(OnSyncAll, Maybe<IObservable<bool>>.None);
        IsBusy = SyncAll.IsExecuting;
    }

    public string Description => $"{source} => {destination}";

    public StoppableCommand<Unit, Result> SyncAll { get; }

    [Reactive] public bool SkipIdentical { get; set; } = true;

    [Reactive] public bool DeleteNonExistent { get; set; } = false;

    [Reactive] public bool CanOverwrite { get; set; } = false;

    [Reactive] public List<SyncItemViewModel>? SyncActions { get; set; }

    public IObservable<bool> IsBusy { get; }
    public IObservable<bool> IsValid => this.IsValid();

    private IObservable<Result> OnSyncAll()
    {
        var syncer = Observable.FromAsync(() => SynchronizerFactory.Create(source, destination, new CopyLeftFilesStrategy()))
            .Successes()
            .SelectMany(x => Observable.FromAsync(x.Execute));
        return syncer;
    }
}