using System.Reflection.Metadata.Ecma335;

namespace API;

public class SolutionWords
{
    public HashSet<WordleWord>[] Words = new HashSet<WordleWord>[6];

    public SolutionWords()
    {
        for (int row = 0; row < 6; row++)
        {
            Words[row] = [];
        }
    }

    public HashSet<WordleWord> this[int index]
    {
        get => Words[index];
        set => Words[index] = value;
    }
}
