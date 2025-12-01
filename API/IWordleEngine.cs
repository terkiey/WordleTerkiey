namespace API;

public interface IWordleEngine
{
    DrawingSolutionDTO SolveDrawing(BoardClue userDrawing);
}
