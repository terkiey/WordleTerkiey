using API;
using System.Collections.ObjectModel;

namespace AppWPF;

internal class SolutionToExampleMapper : ISolutionToExampleMapper
{
    public SolutionExampleVM MapSolutionToExample(Solution solution)
    {
        ObservableCollection<ColorBoxVM> colors = [];
        ObservableCollection<string> words = [];

        // iterate over rows to get the colors and words from first possible example.
        for (int rowIndex = 0; rowIndex < 6; rowIndex++)
        {
            words.Add(solution.words[rowIndex].FirstOrDefault()!.ToString());
            
            for (int letterIndex = 0; letterIndex < 5; letterIndex++)
            {
                BoxColor color = solution.outputBoard[rowIndex][letterIndex];
                ColorBoxVM vm = new ColorBoxVM(color);
                colors.Add(vm);
            }
        }
        return new(colors, words);
    }
}
