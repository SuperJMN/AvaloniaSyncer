using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace AvaloniaSyncer.Plugins.SeaweedFS.Configuration;

public partial class EditProfileView : UserControl
{
    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<EditProfileView, ICommand>(
        nameof(Command));

    public static readonly StyledProperty<ProfileViewModel> ProfileProperty = AvaloniaProperty.Register<EditProfileView, ProfileViewModel>(
        nameof(Profile));

    public static readonly StyledProperty<string> CommandTextProperty = AvaloniaProperty.Register<EditProfileView, string>(
        nameof(CommandText));

    public EditProfileView()
    {
        InitializeComponent();
    }

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public ProfileViewModel Profile
    {
        get => GetValue(ProfileProperty);
        set => SetValue(ProfileProperty, value);
    }

    public string CommandText
    {
        get => GetValue(CommandTextProperty);
        set => SetValue(CommandTextProperty, value);
    }
}