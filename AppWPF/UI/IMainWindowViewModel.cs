using System.ComponentModel;

namespace AppWPF;

public interface IMainWindowViewModel : INotifyPropertyChanged
{
    public ISidebarViewModel SidebarVM { get; }
    public ISolutionsBrowserViewModel SolutionsBrowserVM { get; }
    public IDrawingPanelViewModel DrawingPanelVM { get; }

    string WindowTitle { get; }
}
