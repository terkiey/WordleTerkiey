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

        // TODO_HIGH: Make the solve command asynchronous so it doesnt lag the UI.
        SolveCommand = new RelayCommand(
        _ => SolveCommandHandler(),
        _ => SolveCommandAllowed());

        CustomWordCommand = new RelayCommand(
        _ => CustomWordCommandHandler());

        _engine.AnswerWordChanged += AnswerWordChangedHandler;
        _drawingPanel.DrawingChanged += DrawingChangedHandler;

        ColorCycleCommandHandler();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<BoxColor>? ColorCycled;

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

    private bool SolveCommandAllowed()
    {
        return StateChanged && (_engine.ValidateDrawing(_drawingPanel.GetBoard()) == DrawingValidation.Valid);
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
    private void DisplayMessageDialog(string messageText)
    {
        var dialog = new MessageDialog(messageText);
        dialog.Owner = Application.Current.MainWindow;
        dialog.ShowDialog();
    }

    private WordleWord? DisplayWordDialog(string DefaultText, WordDialog dialog)
    {
        dialog.UserInputTextBox.Text = DefaultText;
        dialog.Owner = Application.Current.MainWindow;

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            WordleWord word = new("ERROR");
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
}
