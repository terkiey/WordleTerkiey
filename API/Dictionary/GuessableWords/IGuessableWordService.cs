using CoreObjects;

namespace API;

internal interface IGuessableWordService
{
    HashSet<WordleWord> GuessableWords { get; }
}
