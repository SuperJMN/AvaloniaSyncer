﻿using ReactiveUI;
using System;
using Zafiro.Avalonia.Wizard.Interfaces;

namespace AvaloniaSyncer.Sections.NewSync.Pages;


internal class Page2ViewModel : ReactiveObject, IValidatable
{
    public Page2ViewModel()
    {
    }

    public IObservable<bool> IsValid { get; }
}