namespace API;

internal class WordleEngine : IWordleEngine
{
    private readonly IDrawingSolver _solver;
    public WordleEngine(IDrawingSolver solver)
    {
        _solver = solver;
    }

    public DrawingSolutionDTO SolveDrawing(BoardClue userDrawing)
    {
        return _solver.Solve(userDrawing);
    }
}
