namespace API;

public interface IWordleEngine
{
    event EventHandler<WordleWord>? AnswerWordChanged;
    event EventHandler<DrawingSolutionDTO>? SolutionsReady;

    void SolveDrawing(BoardClue userDrawing);
    DrawingValidation ValidateDrawing(BoardClue userDrawing);
    void ChangeAnswer(WordleWord customAnswer);
}
