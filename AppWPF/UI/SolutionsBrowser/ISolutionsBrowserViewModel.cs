using System.ComponentModel;

namespace AppWPF;

public interface ISolutionsBrowserViewModel : INotifyPropertyChanged
{
    SolutionExampleVM exactSolutionExample { get; }
    SolutionExampleVM shapeSolutionExample { get; }
    SolutionExampleVM missOneSolutionExample { get; }


}
