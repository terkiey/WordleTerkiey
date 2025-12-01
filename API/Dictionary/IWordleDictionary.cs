namespace API;

internal interface IWordleDictionary
{
    HashSet<WordleWord> AllowedWords { get; }
    WordleWord AnswerWord { get; }
    void UpdateAnswerWord(WordleWord answerWord);
}
