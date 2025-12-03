namespace API;

public class WordClue : IEquatable<WordClue>
{
    public BoxColor[] LetterClues = new BoxColor[5];

    public WordClue() { }

    public WordClue(BoxColor[] letterClues)
    {
        LetterClues = letterClues;
    }

    public WordClue(WordClue original)
    {
        LetterClues = new BoxColor[original.LetterClues.Length];
        Array.Copy(original.LetterClues, LetterClues, original.LetterClues.Length);
    }
    
    public bool Equals(WordClue? otherWordClue)
    {
        if (otherWordClue is null) return false;
        return LetterClues.SequenceEqual(otherWordClue.LetterClues);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as WordClue);
    }

    public static bool operator ==(WordClue a, WordClue b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.LetterClues.SequenceEqual<BoxColor>(b.LetterClues);
    }

    public static bool operator !=(WordClue a, WordClue b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        foreach (BoxColor color in LetterClues)
        {
            hash.Add(color.GetHashCode());
        }
        return hash.ToHashCode();
    }

    public BoxColor this[int index]
    {
        get => LetterClues[index];
        set => LetterClues[index] = value;
    }

    public int CountGreen()
    {
        int count = 0;
        foreach(BoxColor box in LetterClues)
        {
            if (box == BoxColor.Green) count++;
        }
        return count;
    }

    public int CountYellow()
    {
        int count = 0;
        foreach (BoxColor box in LetterClues)
        {
            if (box == BoxColor.Yellow) count++;
        }
        return count;
    }
}
