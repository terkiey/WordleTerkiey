namespace API;

internal interface IWordleDictionary
{
    HashSet<WordleWord> AllowedWords { get; }
}
