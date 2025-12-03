namespace API;

internal interface IBoardMapper
{
    List<BoardClue> MapToShape(BoardClue boardClue);
    List<BoardClue> MapToMissOne(BoardClue boardClue);
}