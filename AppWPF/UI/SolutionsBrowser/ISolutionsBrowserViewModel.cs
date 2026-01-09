using System.ComponentModel;

namespace AppWPF;

public interface ISolutionsBrowserViewModel : INotifyPropertyChanged
{
    SolutionExampleVM ExactSolutionExample { get; }
    SolutionExampleVM ShapeSolutionExample { get; }
    SolutionExampleVM MirrorPaletteSolutionExample { get; }
}
