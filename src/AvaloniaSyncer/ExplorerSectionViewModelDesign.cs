using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using AvaloniaSyncer.Sections.Connections.Configuration.Sftp;
using AvaloniaSyncer.Sections.Explorer;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;

namespace AvaloniaSyncer;

public class ExplorerSectionViewModelDesign : IExplorerSectionViewModel
{
    public ExplorerSectionViewModelDesign()
    {
        Connections = new ReadOnlyObservableCollection<IZafiroFileSystemConnectionViewModel>(new ObservableCollection<IZafiroFileSystemConnectionViewModel>(new List<IZafiroFileSystemConnectionViewModel>()
        {
            (IZafiroFileSystemConnectionViewModel)new FileSystemConnectionDesign("Test1"),
            (IZafiroFileSystemConnectionViewModel)new FileSystemConnectionDesign("Test2"),
            (IZafiroFileSystemConnectionViewModel)new FileSystemConnectionDesign("Long title for this tab item"),
        }));
    }

    public ReadOnlyObservableCollection<IZafiroFileSystemConnectionViewModel> Connections { get; }
}

public class FileSystemConnectionDesign : IZafiroFileSystemConnectionViewModel
{
    public FileSystemConnectionDesign(string name)
    {
        Name = name;
    }

    public ReactiveCommand<Unit, Result<IDisposableFilesystemRoot>> Load { get; set; }
    public string Name { get; }
    public ReactiveCommand<Unit, Result<IDisposableFilesystemRoot>> Refresh { get; set; }
}