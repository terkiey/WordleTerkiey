namespace API;

internal interface IWordleDictionary
{
    HashSet<WordleWord> AllowedWords { get; }
    WordleWord AnswerWord { get; }

    event EventHandler<WordleWord>? AnswerWordChanged;

    void UpdateAnswerWord(WordleWord answerWord);
}
