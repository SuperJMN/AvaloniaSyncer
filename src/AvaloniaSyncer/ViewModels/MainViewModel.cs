using System.Collections.Generic;

namespace AvaloniaSyncer.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(IEnumerable<Section> sections)
    {
        Sections = sections;
    }

    public IEnumerable<Section> Sections { get; set; }
}