using System.Reflection;

namespace API;

internal class GuessableWordService : IGuessableWordService
{
    public HashSet<WordleWord> GuessableWords { get; init; }
    public GuessableWordService()
    {
        GuessableWords = ReadTxtWords();
    }

    private static HashSet<WordleWord> ReadTxtWords()
    {
        HashSet<WordleWord> guessableWords = [];
        string path = "API.Dictionary.GuessableWords.guessableWords.txt";
        string text = ReadEmbeddedText(path);

        string[] words = text.Split(' ');
        foreach (string word in words)
        {
            WordleWord wordleWord = new(word);
            guessableWords.Add(wordleWord);
        }

        return guessableWords;
    }

    private static string ReadEmbeddedText(string path)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using Stream stream = assembly.GetManifestResourceStream(path)
            ?? throw new InvalidOperationException($"Resource not found: {path}");

        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
