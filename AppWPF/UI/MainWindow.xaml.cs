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

    public new void Show()
    {
        // Call original show method
        base.Show();

        // Now prompt the user to input today's answer.
        var sidebarVM = (SidebarViewModel)Sidebar.DataContext;
        sidebarVM.PromptAnswerInput();
    }
}