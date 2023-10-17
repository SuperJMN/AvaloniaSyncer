using Avalonia.Data.Converters;
using ByteSizeLib;

namespace AvaloniaSyncer;

public static class ByteConverters
{
    public static FuncValueConverter<ByteSize, string> ByteRateConverter = new FuncValueConverter<ByteSize, string>(size => size + "/s");
}