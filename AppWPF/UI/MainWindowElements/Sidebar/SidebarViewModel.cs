using API;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace AppWPF;

public class SidebarViewModel : ISidebarViewModel
{
    private readonly IDrawingPanelViewModel _drawingPanel;
    private readonly IWordleEngine _engine;

    public ICommand ClearCommand { get; }
    public ICommand ColorCycleCommand { get; }
    public ICommand SolveCommand { get; }
    public ICommand CustomWordCommand { get; }

    private bool StateChanged = true;

    private bool _drawingValidity = true;
    private bool DrawingValidityState 
    {
        get { return _drawingValidity; }
        set { _drawingValidity = value; }
    }

    public BoxColor ColorPicker { get; private set; }

    public SidebarViewModel(IDrawingPanelViewModel drawingPanel, IWordleEngine engine)
    {
        _drawingPanel = drawingPanel;
        _engine = engine;

        ClearCommand = new RelayCommand(
        _ => ClearCommandHandler(),
        _ => ClearCommandAllowed());

        ColorCycleCommand = new RelayCommand(
        _ => ColorCycleCommandHandler());

        SolveCommand = new RelayCommand(async
        _ => await SolveCommandHandlerAsync(),
        _ => SolveCommandAllowed());

        CustomWordCommand = new RelayCommand(
        _ => CustomWordCommandHandler());

        _engine.AnswerWordChanged += AnswerWordChangedHandler;
        _drawingPanel.DrawingChanged += DrawingChangedHandler;
        _drawingPanel.DrawingValidityChecked += DrawingValidityCheckedHandler;

        ColorCycleCommandHandler();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<BoxColor>? ColorCycled;

    public void PromptAnswerInput()
    {
        CustomWordCommandHandler();
    }

    // ICommand Relays
    private void ClearCommandHandler()
    {
        _drawingPanel.ClearBoard();
    }

    private bool ClearCommandAllowed()
    {
        return !_drawingPanel.IsBoardEmpty();
    }

    private void ColorCycleCommandHandler()
    {
        ColorPicker = _drawingPanel.CycleColor();
        PropertyChanged?.Invoke(this, new(nameof(ColorPicker)));
        ColorCycled?.Invoke(this, ColorPicker);
    }

    private void SolveCommandHandler()
    {
        BoardClue userDrawing = _drawingPanel.GetBoard();
        _engine.SolveDrawing(userDrawing);

        StateChanged = false;
        PropertyChanged?.Invoke(this, new(nameof(StateChanged)));
    }

    private async Task SolveCommandHandlerAsync()
    {
        StateChanged = false;
        PropertyChanged?.Invoke(this, new(nameof(StateChanged)));
        _drawingPanel.UpdateSolveState(SolveState.Solving);

        try
        {
            BoardClue userDrawing = _drawingPanel.GetBoard();
            await Task.Run(() => _engine.SolveDrawing(userDrawing));
        }
        finally
        {
            _drawingPanel.UpdateSolveState(SolveState.Done);
        }
    }

    private bool SolveCommandAllowed()
    {
        return StateChanged && DrawingValidityState;
    }

    private void CustomWordCommandHandler()
    {
        var dialog = new WordDialog();
        WordleWord? customWord = DisplayWordDialog("", dialog);
        if (customWord is null)
        {
            return;
        }

        _engine.ChangeAnswer(customWord);
    }

    // Dialog Openers
    private static WordleWord? DisplayWordDialog(string DefaultText, WordDialog dialog)
    {
        dialog.UserInputTextBox.Text = DefaultText;
        dialog.Owner = Application.Current.MainWindow;

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            WordleWord word;
            string? userValue = dialog.UserInput;
            if (userValue is null) { return null; }
            try
            {
                word = new(userValue);
            }
            catch
            {
                return DisplayWordDialog("The input supplied was not a valid WordleWord.", new());
            }

            return word;
        }

        return null;
    }

    // Event Handlers
    private void AnswerWordChangedHandler(object? sender, WordleWord answerWord)
    {
        StateChanged = true;
        PropertyChanged?.Invoke(this, new(nameof(StateChanged)));
    }

    private void DrawingChangedHandler(object? sender, EventArgs e)
    {
        StateChanged = true;
        PropertyChanged?.Invoke(this, new(nameof(StateChanged)));
    }

    private void DrawingValidityCheckedHandler(object? sender, DrawingValidation validation)
    {
        // This is where the sidebar receives the drawing validity state (from the drawing panel event) and updates the retained state info so the solve button can reflect that.
        if (validation == DrawingValidation.Valid)
        {
            DrawingValidityState = true;
        }
        else
        {
            DrawingValidityState = false;
        }
        CommandManager.InvalidateRequerySuggested();
    }
}
