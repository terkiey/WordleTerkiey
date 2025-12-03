using System.Windows.Controls;

namespace AppWPF;

/// <summary>
/// Interaction logic for SolutionsBrowserControl.xaml
/// </summary>
public partial class SolutionsBrowserControl : UserControl
{
    public SolutionsBrowserControl()
    {
        InitializeComponent();
    }

    /* TODO_MID: This should conditionally show the user control to show the a category example (put those into user controls btw),
     * when no example is available, it should indicate as such with a different user control that it swaps in place to show that class of solution is impossible.
     */

}
