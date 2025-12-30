using API;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AppWPF;

public class DrawingPanelViewModel : IDrawingPanelViewModel
{
    private readonly IWordleEngine _engine;

    private BoxColor DrawingColor = BoxColor.Yellow;
    private string _answerHeader = "";

    // drawing validation stuff


    // solver progress stuff
    private SolveState _solveState = SolveState.None;
    private DispatcherTimer? _progressTimer;
    private int _solveProgressIndex = 0;
    private readonly string[] _progressStates = { "", ".", "..", "..." };
    private string _progressText => _progressStates[_solveProgressIndex];

    public string AnswerHeader 
    { 
        get { return _answerHeader; }
        private set
        {
            _answerHeader = value;
            PropertyChanged?.Invoke(this, new(nameof(AnswerHeader)));
        }
    }

    public ObservableCollection<DrawingGridCell> Cells { get; private set; } = [];
    public SolveState SolveState
    {
        get { return _solveState; }
        private set
        {
            _solveState = value;
            PropertyChanged?.Invoke(this, new(nameof(SolveState)));
            PropertyChanged?.Invoke(this, new(nameof(SolveStateText)));
        }
    }

    public string SolveStateText
    {
        get { return TranslateSolveState(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<EventArgs>? DrawingChanged;

    /*
    public ICommand ColorBoxInCommand { get; }
    public ICommand ClearBoxCommand { get; }
    */

    public AsyncRelayCommand<DrawingGridCell> ColorBoxInCommand { get; }
    public AsyncRelayCommand<DrawingGridCell> ClearBoxCommand { get; }

    public DrawingPanelViewModel(IWordleEngine engine)
    {
        _engine = engine;
        _engine.AnswerWordChanged += AnswerWordChangedHandler;
        _engine.ChangeAnswer(new("dumpy"));
        
        InitialiseCells();


        ColorBoxInCommand = new AsyncRelayCommand<DrawingGridCell>(ColorBoxInCommandHandlerAsync);
        ClearBoxCommand = new AsyncRelayCommand<DrawingGridCell>(ClearBoxCommandHandlerAsync);

        /*
        ColorBoxInCommand = new RelayCommand(
        parameters => ColorBoxInCommandHandler((DrawingGridCell)parameters!));

        ClearBoxCommand = new RelayCommand(
        parameters => ClearBoxCommandHandler((DrawingGridCell)parameters!));
        */

        DrawingChanged += (_, _) => UpdateSolveState(SolveState.None);
    }

    public async Task ColorBoxInCommandHandlerAsync(DrawingGridCell? cell)
    {
        await Task.Yield();

        if (cell == null) { return; }
        cell.Color = DrawingColor;
        DrawingChanged?.Invoke(this, EventArgs.Empty);
        PropertyChanged?.Invoke(this, new(nameof(Cells)));
    }

    public async Task ClearBoxCommandHandlerAsync(DrawingGridCell? cell)
    {
        await Task.Yield();    
    
        if (cell == null) { return; }
        cell.Color = BoxColor.Black;
        DrawingChanged?.Invoke(this, EventArgs.Empty);
        PropertyChanged?.Invoke(this, new(nameof(Cells)));
    }

    public void ColorBoxInCommandHandler(DrawingGridCell cell)
    {
        cell.Color = DrawingColor;
        DrawingChanged?.Invoke(this, EventArgs.Empty);
        PropertyChanged?.Invoke(this, new(nameof(Cells)));
    }

    public void ClearBoxCommandHandler(DrawingGridCell cell)
    {
        cell.Color = BoxColor.Black;
        DrawingChanged?.Invoke(this, EventArgs.Empty);
        PropertyChanged?.Invoke(this, new(nameof(Cells)));
    }

    public BoardClue GetBoard()
    {
        WordClue[] clueArray = new WordClue[6];
        for (int wordIndex = 0; wordIndex < 6; wordIndex++)
        {
            clueArray[wordIndex] = new WordClue();
            for (int letterIndex = 0; letterIndex < 5; letterIndex++)
            {
                int Coordinate = (wordIndex * 5) + letterIndex;
                clueArray[wordIndex][letterIndex] = Cells[Coordinate].Color;
            }
        }

        return new(clueArray);
    }

    public bool IsBoardEmpty()
    {
        foreach (DrawingGridCell cell in Cells)
        {
            if (cell.Color != default)
            {
                return false;
            }
        }

        return true;
    }

    public void ClearBoard()
    {
        Cells.Clear();
        InitialiseCells();

        DrawingChanged?.Invoke(this, EventArgs.Empty);
    }

    public BoxColor CycleColor()
    {
        if (DrawingColor == BoxColor.Yellow)
        {
            DrawingColor = BoxColor.Green;
        }
        else
        {
            DrawingColor = BoxColor.Yellow;
        }
        PropertyChanged?.Invoke(this, new(nameof(DrawingColor)));
        return DrawingColor;
    }

    public void UpdateSolveState(SolveState state)
    {
        if (SolveState == SolveState.Solving && state != SolveState.Solving)
        {
            StopSolvingAnimation();
            SolveState = state;
            return;
        }

        if (state == SolveState.Solving)
        {
            SolveState = state;
            StartSolvingAnimation();
            return;
        }
    }

    public void UserDrawingDragInputFinished()
    {
        CheckDrawingValidity();
    }

    private void CheckDrawingValidity()
    {
        DrawingValidation validation = _engine.ValidateDrawing(GetBoard());
        switch (validation)
        {
            case DrawingValidation.Unspecified:
                break;
        }
        // TODO_HIGH: Finish this, incorporate so the solve button state actually reflects the drawing validity again properly.
    }

    private string TranslateSolveState()
    {
        switch (_solveState)
        {
            case SolveState.None:
                return "Ready to solve";

            case SolveState.Solving:
                return "Currently solving" + _progressText;

            case SolveState.Done:
                return "Solve request completed!";

            default:
                return "something is fucked";
        }
    }

    private void StartSolvingAnimation()
    {
        _progressTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _progressTimer.Tick += (s, e) =>
        {
            _solveProgressIndex = (_solveProgressIndex + 1) % _progressStates.Length;
            PropertyChanged?.Invoke(this, new(nameof(SolveStateText)));
        };
        _progressTimer.Start();
    }

    private void StopSolvingAnimation()
    {
        _progressTimer?.Stop();
        _progressTimer = null;
        _solveProgressIndex = 0;
    }

    private void AnswerWordChangedHandler(object? sender, WordleWord answerWord)
    {
        string AnswerString = answerWord.ToString();
        AnswerHeader = "Today's Wordle: " + AnswerString;
        SolveState = SolveState.None;
    }

    private void InitialiseCells()
    {
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                Cells.Add(new DrawingGridCell(row, col));
            }
        }
    }
}
