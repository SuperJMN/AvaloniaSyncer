using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace AvaloniaSyncer.Controls;

public class StringProperty : ReactiveValidationObject
{
    public StringProperty(string value)
    {
        Value = value;
        Temp = value;
        Submit = ReactiveCommand.Create(() => Value = Temp, this.IsValid());
        Cancel = ReactiveCommand.Create(() => Temp = Value);
    }

    [Reactive]
    public string Temp { get; set; }

    [Reactive]
    public string Value { get; set; }

    public ICommand Submit { get; }
    public ICommand Cancel { get; set; }
}