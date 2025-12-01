namespace API;

public class WordClue : IEquatable<WordClue>
{
    public BoxColor[] LetterClues = new BoxColor[5];

    public WordClue() { }

    public WordClue(BoxColor[] letterClues)
    {
        LetterClues = letterClues;
    }
    
    public bool Equals(WordClue? otherWordClue)
    {
        if (otherWordClue == null) return false;
        if (!otherWordClue.LetterClues.Equals(LetterClues)) return false;
        return true;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as WordClue);
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
