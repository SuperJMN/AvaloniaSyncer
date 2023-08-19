using System.Collections.Generic;
using Avalonia;
using ReactiveUI.Fody.Helpers;

namespace AvaloniaSyncer.ViewModels;

public class MainViewModel : ViewModelBase
{
    public IEnumerable<Section> Sections { get; } = new List<Section>()
    {
        new("Synchronize", ViewModelFactory.GetSyncViewModel(Application.Current!.ApplicationLifetime!)),
        new("Settings", ViewModelFactory.GetSettingsViewModel())
    };

    [Reactive]
    public object SelectedItem { get; set; }
}