using System.Numerics;

namespace API;

public class BoardClue : IEquatable<BoardClue>
{
    private const int WordCount = 6;
    public WordClue[] WordClueArray = Enumerable.Range(0, WordCount)
                                                .Select(_ => new WordClue())
                                                .ToArray();

    public BoardClue() { }

    public BoardClue(WordClue[] wordClues)
    {
        WordClueArray = wordClues; 
    }

    public BoardClue(BoardClue original)
    {
        WordClueArray = new WordClue[original.WordClueArray.Length];
        for (int wordIndex = 0; wordIndex < original.WordClueArray.Length; wordIndex++)
        {
            WordClue wordClue = new(original.WordClueArray[wordIndex]);
            WordClueArray[wordIndex] = wordClue;
        }
    }

    public bool Equals(BoardClue? otherBoardClue)
    {
        if (otherBoardClue == null) return false;
        if (!otherBoardClue.WordClueArray.SequenceEqual(WordClueArray)) return false;
        return true;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as BoardClue);
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        foreach (WordClue wordClue in WordClueArray)
        {
            hash.Add(wordClue.GetHashCode());
        }
        return hash.ToHashCode();
    }

    public WordClue this[int index]
    {
        get => WordClueArray[index];
        set => WordClueArray[index] = value;
    }

    public static bool operator ==(BoardClue a, BoardClue b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.WordClueArray.SequenceEqual<WordClue>(b.WordClueArray);
    }

    public static bool operator !=(BoardClue a, BoardClue b)
    {
        return !(a == b);
    }

}
