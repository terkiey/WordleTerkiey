using System.ComponentModel;

namespace AppWPF;

public class MainWindowViewModel : IMainWindowViewModel
{
    public ISidebarViewModel SidebarVM { get; }
    public IDrawingPanelViewModel DrawingPanelVM { get; }
    public ISolutionsBrowserViewModel SolutionsBrowserVM { get; }

    public MainWindowViewModel(ISidebarViewModel sidebarViewModel, ISolutionsBrowserViewModel solutionsBrowserViewModel, IDrawingPanelViewModel drawingPanelViewModel)
    {
        SidebarVM = sidebarViewModel;
        SolutionsBrowserVM = solutionsBrowserViewModel;
        DrawingPanelVM = drawingPanelViewModel;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
