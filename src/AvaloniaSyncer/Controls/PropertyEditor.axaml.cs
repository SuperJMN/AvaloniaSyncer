using Avalonia;
using Avalonia.Controls.Primitives;

namespace AvaloniaSyncer.Controls
{
    public class StringEditor : TemplatedControl
    {
        public static readonly StyledProperty<StringProperty> StringPropertyProperty = AvaloniaProperty.Register<StringEditor, StringProperty>(
            nameof(StringProperty));

        public StringProperty StringProperty
        {
            get => GetValue(StringPropertyProperty);
            set => SetValue(StringPropertyProperty, value);
        }
    }
}
