using API;
using System.ComponentModel;

namespace AppWPF;

public class MainWindowViewModel : IMainWindowViewModel, INotifyPropertyChanged
{
    private readonly IWordleEngine _engine;

    public ISidebarViewModel SidebarVM { get; }
    public IDrawingPanelViewModel DrawingPanelVM { get; }
    public ISolutionsBrowserViewModel SolutionsBrowserVM { get; }

    private string WindowAnswer = String.Empty;

    public string WindowTitle 
    { 
        get
        {
            return "WordleTerkiey" + WindowAnswer;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainWindowViewModel(IWordleEngine engine, ISidebarViewModel sidebarViewModel, ISolutionsBrowserViewModel solutionsBrowserViewModel, IDrawingPanelViewModel drawingPanelViewModel)
    {
        _engine = engine;
        _engine.AnswerWordChanged += AnswerWordChangedHandler;

        SidebarVM = sidebarViewModel;
        SolutionsBrowserVM = solutionsBrowserViewModel;
        DrawingPanelVM = drawingPanelViewModel;

        SolutionsBrowserVM.SolutionRequested += SolutionRequestedHandler;
        SolutionsBrowserVM.NewDrawing += NewDrawingHandler;
    }

    private void AnswerWordChangedHandler(object? sender, WordleWord answerWord)
    {
        WindowAnswer = " - " + answerWord.ToString();
        PropertyChanged?.Invoke(this, new(nameof(WindowTitle)));
    }

    private void SolutionRequestedHandler(object? sender, BoardClue board)
    {
        SidebarVM.SolveCommand.Execute(board);
    }

    private void NewDrawingHandler(object? sender, BoardClue drawing)
    {
        DrawingPanelVM.LoadBoard(drawing);
    }
}
