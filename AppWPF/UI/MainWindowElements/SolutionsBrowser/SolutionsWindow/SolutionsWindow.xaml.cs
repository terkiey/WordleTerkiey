using API;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AppWPF;
/// <summary>
/// Interaction logic for SolutionsWindow.xaml
/// </summary>
public partial class SolutionsWindow : Window
{
    public ICommand CloseThisWindow { get; }

    public SolutionsWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;

        CloseThisWindow = new RelayCommand(
            _ => Close(),
            null);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is SolutionsBrowserViewModel vm)
        {
            vm.CloseSolutionBrowser += (_, _) => Close();
        }
    }
}
