namespace API;

internal class WordleDictionary : IWordleDictionary
{
    private readonly IGuessableWordService _guessableWordService;
    private readonly IAnswerWordService _answerWordService;
    
    public HashSet<WordleWord> AllowedWords { get; private set; }
    public WordleWord AnswerWord { get; private set; }

    public event EventHandler<WordleWord>? AnswerWordChanged;

    public WordleDictionary(IGuessableWordService guessableWordService, IAnswerWordService answerWordService)
    {
        _guessableWordService = guessableWordService;
        _answerWordService = answerWordService;

        AllowedWords = _guessableWordService.GuessableWords;
        AnswerWord = _answerWordService.Answer;
        _answerWordService.AnswerWordChanged += HandleAnswerWordChanged;
    }

    public void UpdateAnswerWord(WordleWord answerWord)
    {
        _answerWordService.Answer = answerWord;
    }

    private void HandleAnswerWordChanged(object? sender, WordleWord answerWord)
    {
        AnswerWord = answerWord;
        AllowedWords.Add(answerWord);
        AnswerWordChanged?.Invoke(this, answerWord);
    }
}