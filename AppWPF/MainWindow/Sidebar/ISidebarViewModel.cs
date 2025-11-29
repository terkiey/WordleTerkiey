using System.ComponentModel;

namespace AppWPF;

public interface ISidebarViewModel : INotifyPropertyChanged
{
    public event EventHandler<BoxColor>? ColorCycled;
}
