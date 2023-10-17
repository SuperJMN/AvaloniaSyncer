using System;
using ReactiveUI;
using Zafiro.Avalonia.WizardOld.Interfaces;

namespace AvaloniaSyncer.Sections.Syncronization.Pages;

internal class Page1ViewModel : ReactiveObject, IValidatable
{
    public Page1ViewModel()
    {
        
    }

    public IObservable<bool> IsValid { get; }
}