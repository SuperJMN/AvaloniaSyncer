﻿using System;
using ReactiveUI;
using Zafiro.Avalonia.WizardOld.Interfaces;

namespace AvaloniaSyncer.Sections.NewSync.Pages;

internal class Page1ViewModel : ReactiveObject, IValidatable
{
    public Page1ViewModel()
    {
        
    }

    public IObservable<bool> IsValid { get; }
}