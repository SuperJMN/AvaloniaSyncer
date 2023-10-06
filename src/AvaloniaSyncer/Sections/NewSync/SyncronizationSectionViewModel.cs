using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Controls;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Wizard;
using Zafiro.Avalonia.Wizard.Interfaces;

namespace AvaloniaSyncer.Sections.NewSync;

public class SyncronizationSectionViewModel : ReactiveObject
{
    private readonly IDialogService dialogService;

    public SyncronizationSectionViewModel(ReadOnlyObservableCollection<FileSystemConnectionViewModel> connections, IDialogService dialogService)
    {
        this.dialogService = dialogService;
        AddSession = ReactiveCommand.CreateFromTask(OnAddSession);
    }

    public ReactiveCommand<Unit, Maybe<Session>> AddSession { get; set; }

    public Task<Maybe<Session>> OnAddSession()
    {
        var wizard = CreateWizard();
        return ShowWizard(wizard);
    }

    private SuperWizard<Page1, Page2, Session> CreateWizard()
    {
        var pages = new List<IPage<IValidatable, IValidatable>>();
        var wizard = new SuperWizard<Page1, Page2, Session>(() => new Page1(), page1 => new Page2(page1));
        return wizard;
    }

    private async Task<Maybe<Session>> ShowWizard(SuperWizard<Page1, Page2, Session> wizard)
    {
        var showDialog = await dialogService.ShowDialog<SuperWizard<Page1, Page2, Session>, Session>(wizard, "Do something, boi", w => Observable.FromAsync(() => w.Result));
        return showDialog;
    }
}

internal class Page2 : ReactiveObject, IValidatable
{
    public Page2(Page1 page1)
    {
    }

    public IObservable<bool> IsValid { get; }
}

internal class Page1 : ReactiveObject, IValidatable
{
    public IObservable<bool> IsValid { get; }
}

public class Session
{
}