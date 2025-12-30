using CommunityToolkit.Mvvm.Input;
using System.Buffers;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace AppWPF;
/// <summary>
/// Interaction logic for WordDialog.xaml
/// </summary>
public partial class WordDialog : Window, INotifyPropertyChanged
{
    string? _userInput;

    public string? UserInput
    {
        get => _userInput;
        set
        {
            _userInput = value;
            PropertyChanged?.Invoke(this, new(nameof(UserInput)));
            OkCommand?.NotifyCanExecuteChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public CommunityToolkit.Mvvm.Input.RelayCommand OkCommand { get; }
    public CommunityToolkit.Mvvm.Input.RelayCommand CancelCommand { get; }

    public WordDialog()
    {
        OkCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(
                    OkCommandHandler,
                    OkAllowed);

        CancelCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(
                        CancelCommandHandler);

        UserInput = "Type your 5 letter word here!";

        InitializeComponent();

        DataContext = this;

        Loaded += WordDialogLoaded;
    }

    private void OkCommandHandler()
    {
        UserInput = UserInputTextBox.Text;
        DialogResult = true;
    }

    private bool OkAllowed()
    {
        return !string.IsNullOrWhiteSpace(UserInput) && UserInput.Length == 5;
    }

    private void CancelCommandHandler()
    {
        UserInput = null;
        DialogResult = true;
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        UserInputTextBox.Focus();
    }

    private void WordDialogLoaded(object sender, RoutedEventArgs e)
    {
        // Focus the textbox
        UserInputTextBox.Focus();

        // Select all text
        UserInputTextBox.SelectAll();
    }
}
