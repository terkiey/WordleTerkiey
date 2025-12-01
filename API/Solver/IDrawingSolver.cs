namespace API;

internal interface IDrawingSolver
{
    DrawingSolutionDTO Solve(BoardClue userDrawing);
}
