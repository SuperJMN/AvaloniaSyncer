using Avalonia.Data.Converters;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.ZafiroHelpers;

public class MiscConverters
{
    public static FuncValueConverter<Maybe<IZafiroFile>, string> MaybeZafiroFileToPath => new(file => file.Map(r => r.Path.ToString()).GetValueOrDefault());
}