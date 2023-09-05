using System;
using System.Collections.Generic;
using AvaloniaSyncer.Plugins;
using ReactiveUI.Validation.Abstractions;
using Zafiro.Avalonia.Model;

namespace AvaloniaSyncer.ViewModels;

public class SessionWizardViewModel : ViewModelBase
{
    public SessionWizardViewModel(IEnumerable<IPlugin> plugins)
    {
        var wizardPageContainer = new PluginSelectionViewModel("Source", plugins);

        this.Wizard = new Wizard(new List<IWizardPage>()
        {
            new WizardPageContainer(new RxUiValidationWizardPageContainer(wizardPageContainer), "Continue")
        });
    }

    public Wizard Wizard { get; }
}

public class RxUiValidationWizardPageContainer : IValidatable
{
    private readonly IValidatableViewModel validatableViewModel;

    public RxUiValidationWizardPageContainer(IValidatableViewModel validatableViewModel)
    {
        this.validatableViewModel = validatableViewModel;
    }

    public IObservable<bool> IsValid => validatableViewModel.ValidationContext.Valid;
}