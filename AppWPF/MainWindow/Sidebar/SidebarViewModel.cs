using System.ComponentModel;

namespace AppWPF;

public class SidebarViewModel : ISidebarViewModel
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<BoxColor>? ColorCycled;
}
