using API;

namespace API;

internal class WordleDictionary : IWordleDictionary
{
    private readonly IGuessableWordService _guessableWordService;
    private readonly IAnswerWordService _answerWordService;

    public HashSet<WordleWord> AllowedWords { get; init; } = [];
    public WordleDictionary(IGuessableWordService guessableWordService, IAnswerWordService answerWordService)
    {
        _guessableWordService = guessableWordService;
        _answerWordService = answerWordService;

        InitialiseAllowedWords();
        InitialiseSubscriptions();
    }

    private void InitialiseAllowedWords()
    {
        HashSet<WordleWord> guessableWords = _guessableWordService.GuessableWords;
        foreach (WordleWord word in guessableWords)
        {
            AllowedWords.Add(word);
        }

        WordleWord answerWord = _answerWordService.Answer;
    }

    private void UpdateAllowedWords(object? sender, WordleWord newAnswer)
    {
        AllowedWords.Add(newAnswer);
    }

    private void InitialiseSubscriptions()
    {
        _answerWordService.AnswerWordChanged += UpdateAllowedWords;
    }
}
