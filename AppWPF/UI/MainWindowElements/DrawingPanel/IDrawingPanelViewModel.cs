using API;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace AppWPF;

public interface IDrawingPanelViewModel : INotifyPropertyChanged
{
    string AnswerHeader { get; }
    ObservableCollection<DrawingGridCell> Cells { get; }
    string SolveStateText { get; }

    event EventHandler<EventArgs>? DrawingChanged;
    event EventHandler<DrawingValidation>? DrawingValidityChecked;

    AsyncRelayCommand<DrawingGridCell> ColorBoxInCommand { get; }
    AsyncRelayCommand<DrawingGridCell> ClearBoxCommand { get; }

    /*
    ICommand ColorBoxInCommand { get; }
    ICommand ClearBoxCommand { get; }
    */

    BoardClue GetBoard();
    bool IsBoardEmpty();
    void ClearBoard();
    BoxColor CycleColor();
    void UpdateSolveState(SolveState state);
    void UserDrawingDragInputFinished();
    void LoadBoard(BoardClue boardClue);
}
