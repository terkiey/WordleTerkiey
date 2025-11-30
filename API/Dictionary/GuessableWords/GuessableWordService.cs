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
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "guessableWords.txt");
        string text = File.ReadAllText(path);

        string[] words = text.Split(' ');
        foreach (string word in words)
        {
            WordleWord wordleWord = new(word);
            guessableWords.Add(wordleWord);
        }

        return guessableWords;
    }
}
