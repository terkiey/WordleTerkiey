using CoreObjects;

namespace API;

public interface IWordleDictionary
{
    HashSet<WordleWord> AllowedWords { get; }
}
