using API;
using System.ComponentModel;
using System.Windows.Input;

namespace AppWPF;

public interface ISidebarViewModel : INotifyPropertyChanged
{
    BoxColor ColorPicker { get; }

    event EventHandler<BoxColor>? ColorCycled;

    ICommand ClearCommand { get; }
    ICommand ColorCycleCommand { get; }
    ICommand SolveCommand { get; }
    ICommand CustomWordCommand { get; }
}
