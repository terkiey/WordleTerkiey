using System.Buffers;
using System.Windows;


namespace AppWPF;
/// <summary>
/// Interaction logic for WordDialog.xaml
/// </summary>
public partial class WordDialog : Window
{
    public string UserInput { get; set; }

    public WordDialog()
    {
        InitializeComponent();

        UserInput = "";
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        UserInput = UserInputTextBox.Text;
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
