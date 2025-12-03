namespace API;

internal class WordleEngine : IWordleEngine
{
    private readonly IDrawingSolver _solver;
    private readonly IWordleDictionary _wordleDictionary;

    public event EventHandler<DrawingSolutionDTO>? SolutionsReady;
    public event EventHandler<WordleWord>? AnswerWordChanged;

    public WordleEngine(IDrawingSolver solver, IWordleDictionary wordleDictionary)
    {
        _solver = solver;
        _wordleDictionary = wordleDictionary;

        _wordleDictionary.AnswerWordChanged += AnswerWordChangedHandler;
    }

    public void SolveDrawing(BoardClue userDrawing)
    {
        DrawingSolutionDTO solutionDTO = _solver.Solve(userDrawing);
        SolutionsReady?.Invoke(this, solutionDTO);
    }

    public DrawingValidation ValidateDrawing(BoardClue userDrawing)
    {
        return _solver.ValidateDrawing(userDrawing);
    }

    public void ChangeAnswer(WordleWord customAnswer)
    {
        _wordleDictionary.UpdateAnswerWord(customAnswer);
    }

    private void AnswerWordChangedHandler(object? sender, WordleWord answerWord)
    {
        AnswerWordChanged?.Invoke(this, answerWord);
    }
}
