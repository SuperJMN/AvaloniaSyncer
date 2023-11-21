using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using AvaloniaSyncer.Sections.Explorer;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;

namespace AvaloniaSyncer;

public class ExplorerSectionViewModelDesign : IExplorerSectionViewModel
{
    public ExplorerSectionViewModelDesign()
    {
        Connections = new ReadOnlyObservableCollection<IFileSystemConnectionViewModel>(new ObservableCollection<IFileSystemConnectionViewModel>(new List<IFileSystemConnectionViewModel>()
        {
            (IFileSystemConnectionViewModel)new FileSystemConnectionDesign("Test1"),
            (IFileSystemConnectionViewModel)new FileSystemConnectionDesign("Test2"),
            (IFileSystemConnectionViewModel)new FileSystemConnectionDesign("Long title for this tab item"),
        }));
    }

    public ReadOnlyObservableCollection<IFileSystemConnectionViewModel> Connections { get; }
}

public class FileSystemConnectionDesign : IFileSystemConnectionViewModel
{
    public FileSystemConnectionDesign(string name)
    {
        Name = name;
    }

    public ReactiveCommand<Unit, Result<IFileSystem>> Load { get; set; }
    public string Name { get; }
    public ReactiveCommand<Unit, Result<IFileSystem>> Refresh { get; set; }
}