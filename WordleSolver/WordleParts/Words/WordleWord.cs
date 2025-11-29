namespace Domain;

public class WordleWord : IEquatable<WordleWord>, IEquatable<string>
{
    public WordleLetter[] WordleLetters = new WordleLetter[5];
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
        return WordleLetters.GetHashCode();
    }
}
