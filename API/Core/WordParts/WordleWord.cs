namespace API;

public class WordleWord : IEquatable<WordleWord>, IEquatable<string>
{
    public WordleLetter[] WordleLetters = new WordleLetter[5];

    public WordleWord(WordleLetter[] wordleLetters)
    {
        WordleLetters = wordleLetters;
    }

    public WordleWord(string word)
    {
        if (word.Length != 5)
        {
            throw new ArgumentOutOfRangeException(nameof(word), "WordleWords must be 5 letters long.");
        }

        int arrayIndex = 0;
        foreach (char character in word)
        {
            WordleLetter wordleLetter = new WordleLetter(character);
            WordleLetters[arrayIndex++] = wordleLetter;
        }
    }

    public bool Equals(WordleWord? otherWordleWord)
    {
        if (otherWordleWord == null) return false;
        if (!otherWordleWord.WordleLetters.SequenceEqual(WordleLetters)) return false;
        return true;
    }

    public bool Equals(string? otherString)
    {
        if (otherString == null) return false;

        WordleWord otherWordleWord;
        try
        {
            otherWordleWord = new(otherString);
        }
        catch
        {
            return false;
        }

        if (!this.Equals(otherWordleWord)) return false;
        return true;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as WordleWord);
    }

    public override int GetHashCode()
    {
        if (WordleLetters == null) return 0;

        HashCode hash = new HashCode();
        foreach (WordleLetter letter in WordleLetters)
        {
            hash.Add(letter.GetHashCode());
        }

        return hash.ToHashCode();
    }

    public WordleLetter this[int index]
    {
        get => WordleLetters[index];
        set => WordleLetters[index] = value;
    }

    public override string ToString()
    {
        string output = "";
        for (int i = 0; i < 5; i++)
        {
            output += WordleLetters[i].ToString();
        }
        return output;
    }
}
