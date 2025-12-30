using System.Diagnostics;
using System.Windows;

namespace AppWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(IMainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        DrawingPanel.UserDrawingDragCompleted += (s, e) =>
        {
            var panelVM = (DrawingPanelViewModel)DrawingPanel.DataContext;
            panelVM.UserDrawingDragInputFinished();
        };
    }
}