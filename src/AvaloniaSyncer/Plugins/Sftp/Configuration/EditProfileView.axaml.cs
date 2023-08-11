using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace AvaloniaSyncer.Plugins.Sftp.Configuration;

public partial class EditProfileView : UserControl
{
    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<EditProfileView, ICommand>(
        nameof(Command));

    public static readonly StyledProperty<SftpInfoViewModel> ProfileProperty = AvaloniaProperty.Register<EditProfileView, SftpInfoViewModel>(
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

    public SftpInfoViewModel Profile
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