using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace AvaloniaSyncer.ViewModels;

public class MainViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<IEnumerable<Section>> sectionsHelper;

    public MainViewModel(Func<Task<IEnumerable<Section>>> sectionsFactory)
    {
        LoadSections = ReactiveCommand.CreateFromTask(sectionsFactory);
        sectionsHelper = LoadSections.ToProperty(this, x => x.Sections);
    }

    public IEnumerable<Section> Sections => sectionsHelper.Value;

    public ReactiveCommand<Unit, IEnumerable<Section>> LoadSections { get; }
}