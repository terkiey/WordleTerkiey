using API;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace AppWPF;

public interface IDrawingPanelViewModel : INotifyPropertyChanged
{
    string AnswerHeader { get; }
    ObservableCollection<DrawingGridCell> Cells { get; }

    event EventHandler<EventArgs>? DrawingChanged;

    ICommand ColorBoxInCommand { get; }
    ICommand ClearBoxCommand { get; }

    BoardClue GetBoard();
    bool IsBoardEmpty();
    void ClearBoard();
    BoxColor CycleColor();
}
