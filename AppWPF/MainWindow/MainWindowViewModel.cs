using System.ComponentModel;

namespace AppWPF;

public class MainWindowViewModel : IMainWindowViewModel
{
    public ISidebarViewModel SidebarViewModel { get; init; }
    public ISolutionsBrowserViewModel SolutionsBrowserViewModel { get; init; }
    public IDrawingPanelViewModel DrawingPanelViewModel { get; init; }

    public MainWindowViewModel(ISidebarViewModel sidebarViewModel, ISolutionsBrowserViewModel solutionsBrowserViewModel, IDrawingPanelViewModel drawingPanelViewModel)
    {
        SidebarViewModel = sidebarViewModel;
        SolutionsBrowserViewModel = solutionsBrowserViewModel;
        DrawingPanelViewModel = drawingPanelViewModel;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
