using API;

namespace API;

public interface IWordleDictionary
{
    HashSet<WordleWord> AllowedWords { get; }
}
