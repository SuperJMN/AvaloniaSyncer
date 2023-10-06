using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Explorer;
using AvaloniaSyncer.Sections.NewSync.Pages;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Controls;
using Zafiro.Avalonia.Dialogs;

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

    private SuperWizard<Page1ViewModel, Page2ViewModel, Session> CreateWizard()
    {
        var wizard = new SuperWizard<Page1ViewModel, Page2ViewModel, Session>(
            new SuperPage<Page1ViewModel>(new Page1ViewModel(), "Next"),
            new SuperPage<Page2ViewModel>(new Page2ViewModel(), "Finish"), (p1, p2) => new Session());
        return wizard;
    }

    private async Task<Maybe<Session>> ShowWizard(SuperWizard<Page1ViewModel, Page2ViewModel, Session> wizard)
    {
        var showDialog = await dialogService.ShowDialog<SuperWizard<Page1ViewModel, Page2ViewModel, Session>, Session>(wizard, "Do something, boi", w => Observable.FromAsync(() => w.Result));
        return showDialog;
    }
}
