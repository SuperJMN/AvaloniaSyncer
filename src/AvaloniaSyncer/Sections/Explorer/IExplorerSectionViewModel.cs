using System.Collections.ObjectModel;

namespace AvaloniaSyncer.Sections.Explorer;

public interface IExplorerSectionViewModel
{
    ReadOnlyObservableCollection<IZafiroFileSystemConnectionViewModel> Connections { get; }
}