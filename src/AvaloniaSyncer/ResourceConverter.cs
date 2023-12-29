using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace AvaloniaSyncer;

public class ResourceConverter : AvaloniaObject, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        
        if (value is string claveRecurso)
        {
            return Source[claveRecurso];
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public static readonly StyledProperty<ResourceDictionary> SourceProperty = AvaloniaProperty.Register<ResourceConverter, ResourceDictionary>(
        "Source");

    public ResourceDictionary Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
}