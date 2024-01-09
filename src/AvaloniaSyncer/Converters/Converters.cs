using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AvaloniaSyncer.Converters;

public class Converters
{
    public static readonly FuncValueConverter<bool, FontWeight> IsDirtyToFontWeight = new(v => v ? FontWeight.Light : FontWeight.Regular);
}