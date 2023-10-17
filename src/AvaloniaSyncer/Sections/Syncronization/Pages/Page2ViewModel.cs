using System;
using ReactiveUI;
using Zafiro.Avalonia.WizardOld.Interfaces;

namespace AvaloniaSyncer.Sections.Syncronization.Pages;


internal class Page2ViewModel : ReactiveObject, IValidatable
{
    public Page2ViewModel()
    {
    }

    public IObservable<bool> IsValid { get; }
}