using System;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using AvaloniaSyncer.Sections.Connections.Configuration;
using AvaloniaSyncer.Sections.Explorer;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Misc;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.ZafiroHelpers;

public static class MiscConverters
{
    public static FuncValueConverter<Maybe<IZafiroFile>, string> MaybeZafiroFileToPath => new(file => file.Map(r => r.Path.ToString()).GetValueOrDefault());
    public static FuncValueConverter<ConfigurationViewModelBase, Control> ConfigurationToIcon => new(CreateImage);
    public static FuncValueConverter<FileSystemConnectionViewModel, Control> ConnectionToIcon => new(CreateImage);

    private static Control CreateImage(ConfigurationViewModelBase? arg)
    {
        var pluginName = arg.GetType().Name.Replace("ConfigurationViewModel", "");

        return new Image
        {
            Source = BitmapFactory.LoadFromResource(new Uri($"avares://AvaloniaSyncer/Plugins/{pluginName}/Icon.png"))
        };
    }

    private static Control CreateImage(FileSystemConnectionViewModel? arg)
    {
        var pluginName = arg.Connection.GetType().Name.Replace("FileSystemConnection", "");

        return new Image
        {
            Source = BitmapFactory.LoadFromResource(new Uri($"avares://AvaloniaSyncer/Plugins/{pluginName}/Icon.png"))
        };
    }
}