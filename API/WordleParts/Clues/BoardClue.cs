namespace API;

public class BoardClue : IEquatable<BoardClue>
{
    public WordClue[] WordClues = new WordClue[6];
    public BoardClue(WordClue[] wordClues)
    {
        WordClues = wordClues; 
    }

    public bool Equals(BoardClue? otherBoardClue)
    {
        if (otherBoardClue == null) return false;
        if (!otherBoardClue.WordClues.SequenceEqual(WordClues)) return false;
        return true;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as BoardClue);
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        foreach (WordClue wordClue in WordClues)
        {
            hash.Add(wordClue.GetHashCode());
        }
        return hash.ToHashCode();
    }
}
