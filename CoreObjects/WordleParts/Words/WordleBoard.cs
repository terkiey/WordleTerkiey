namespace Domain;

public class WordleBoard : IEquatable<WordleBoard>
{
    public WordleWord[] WordleWords = new WordleWord[6];
    public WordleBoard(WordleWord[] wordleWords)
    {
        if (wordleWords.Length > 6)
        {
            throw new ArgumentOutOfRangeException(nameof(wordleWords), "A WordleBoard can have a maximum of 6 words!");
        }

        if (wordleWords.Length < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(wordleWords), "A WordleBoard must have at least 1 word!");
        }

        WordleWords = wordleWords;
    }

    public bool Equals(WordleBoard? otherBoard)
    {
        if (otherBoard == null) return false;
        if (!otherBoard.WordleWords.Equals(WordleWords)) return false;
        return true;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as WordleBoard);
    }

    public override int GetHashCode()
    {
        if (WordleWords == null) return 0;

        HashCode hash = new HashCode();
        foreach (WordleWord word in WordleWords)
        {
            hash.Add(word.GetHashCode());
        }

        return hash.ToHashCode();
    }
}
