using API;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace AppWPF;

public class DrawingPanelViewModel : IDrawingPanelViewModel
{
    private readonly IWordleEngine _engine;
    private BoxColor DrawingColor = BoxColor.Yellow;

    public string AnswerHeader { get; private set; } = "";
    public ObservableCollection<DrawingGridCell> Cells { get; private set; } = [];

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<EventArgs>? DrawingChanged;

    public ICommand ColorBoxInCommand { get; }
    public ICommand ClearBoxCommand { get; }

    public DrawingPanelViewModel(IWordleEngine engine)
    {
        _engine = engine;
        _engine.AnswerWordChanged += AnswerWordChangedHandler;
        _engine.ChangeAnswer(new("dumpy"));
        
        InitialiseCells();

        ColorBoxInCommand = new RelayCommand(
        parameters => ColorBoxInCommandHandler((DrawingGridCell)parameters!));

        ClearBoxCommand = new RelayCommand(
        parameters => ClearBoxCommandHandler((DrawingGridCell)parameters!));
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

    private void AnswerWordChangedHandler(object? sender, WordleWord answerWord)
    {
        string AnswerString = answerWord.ToString();
        AnswerHeader = "Today's Wordle: " + AnswerString;
        PropertyChanged?.Invoke(this, new(nameof(AnswerHeader)));
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
