using System.Xml;

namespace API;

public class WordleLetter : IEquatable<WordleLetter>, IEquatable<char>, IEquatable<string>
{
    private char _value;
    public char Value {
        get => _value;
        set {
            if ((value >= 'a' && value <= 'z') || (value >= 'A' && value <= 'Z'))
            {
                _value = char.ToUpper(value);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Can only set WordleLetters to be Latin letters from A to Z.");
            }
        }
    }

    public WordleLetter(char value)
    {
        this.Value = value;
    }

    public bool Equals(WordleLetter? otherWordleLetter)
    {
        if (otherWordleLetter is null) return false;
        if (otherWordleLetter.Value == this.Value) return true;
        return false;
    }

    public bool Equals(char otherChar)
    {
        if (otherChar == this.Value) return true;
        return false;
    }

    public bool Equals(string? otherString)
    {
        if (otherString is null) return false;
        if (otherString.Length != 1) return false;
        if (otherString[0] == this.Value) return true;
        return false;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as WordleLetter);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        string output = "";
        output += Value.ToString();
        return output;
    }
}
