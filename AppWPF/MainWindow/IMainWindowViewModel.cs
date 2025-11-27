using System.ComponentModel;

namespace AppWPF;

public interface IMainWindowViewModel : INotifyPropertyChanged
{
    public ISidebarViewModel SidebarViewModel { get; init; }
    public ISolutionsBrowserViewModel SolutionsBrowserViewModel { get; init; }
    public IDrawingPanelViewModel DrawingPanelViewModel { get; init; }
}
