using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.UI;

namespace AvaloniaSyncer.ViewModels;

public class MainViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<IEnumerable<Section>> sectionsHelper;

    public MainViewModel(Func<Task<IEnumerable<Section>>> sectionsFactory, ITransferManager transferManager)
    {
        TransferManager = transferManager;
        LoadSections = ReactiveCommand.CreateFromTask(sectionsFactory);
        sectionsHelper = LoadSections.ToProperty(this, x => x.Sections);

        this.WhenAnyValue(model => model.Sections)
            .WhereNotNull()
            .Do(sections => SelectedSection = sections.First())
            .Subscribe();
    }

    public ITransferManager TransferManager { get; }

    [Reactive] public Section SelectedSection { get; set; }

    public IEnumerable<Section> Sections => sectionsHelper.Value;

    public ReactiveCommand<Unit, IEnumerable<Section>> LoadSections { get; }
}