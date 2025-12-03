using System.Buffers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace AppWPF;
/// <summary>
/// Interaction logic for WordDialog.xaml
/// </summary>
public partial class WordDialog : Window
{
    public string? UserInput { get; set; }

    public ICommand OkCommand { get; }
    public ICommand CancelCommand { get; }

    public WordDialog()
    {
        InitializeComponent();

        UserInput = "";

        OkCommand = new RelayCommand(
        _ => OkCommandHandler(),
        _ => OkAllowed());

        CancelCommand = new RelayCommand(
        _ => CancelCommandHandler());

        DataContext = this;
    }

    private void OkCommandHandler()
    {
        UserInput = UserInputTextBox.Text;
        DialogResult = true;
    }

    private bool OkAllowed()
    {
        string currentInput = UserInputTextBox.Text;
        if (currentInput.Trim().Length != 5)
        {
            return false;
        }

        return true;
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
}
