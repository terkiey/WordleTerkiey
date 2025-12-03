using System.Windows;

namespace AppWPF;
/// <summary>
/// Interaction logic for MessageDialog.xaml
/// </summary>
public partial class MessageDialog : Window
{
    public string Message { get; }    

    public MessageDialog(string message)
    {
        InitializeComponent();

        Message = message;
        DataContext = this;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}
