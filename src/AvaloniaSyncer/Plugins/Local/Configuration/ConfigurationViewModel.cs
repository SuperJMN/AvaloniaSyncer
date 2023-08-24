using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace AvaloniaSyncer.Plugins.Local.Configuration;

public class ConfigurationViewModel : ReactiveValidationObject, ISessionConfiguration
{
    public ConfigurationViewModel()
    {
        this.ValidationRule(x => x.Text, s => !string.IsNullOrEmpty(s), "can't be null");
    }

    [Reactive]
    public string Text { get; set; } = "";
}