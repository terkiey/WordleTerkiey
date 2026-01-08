using API;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AppWPF;

public class SolutionExampleVM
{
    public ObservableCollection<ColorBoxVM> Colors { get; set; }
        = new ObservableCollection<ColorBoxVM>(Enumerable.Range(0, 30)
            .Select(_ => new ColorBoxVM(BoxColor.Black)));

    public ObservableCollection<string> Words { get; private set; }

    public SolutionExampleVM() 
    {
        ObservableCollection<string> _words = [];
        for (int i = 0; i < 6; i++)
        {
            _words.Add("");
        }
        Words = _words;
    }

    public SolutionExampleVM(ObservableCollection<ColorBoxVM> colors, ObservableCollection<string> words)
    {
        Colors = colors;
        Words = words;
    }
}
