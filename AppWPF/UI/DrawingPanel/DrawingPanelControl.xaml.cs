using System.Windows.Controls;


namespace AppWPF;
/// <summary>
/// Interaction logic for DrawingPanelControl.xaml
/// </summary>
public partial class DrawingPanelControl : UserControl
{
    public DrawingPanelControl()
    {
        InitializeComponent();
    }
}

/* TODO_HIGH: Indicate live when the drawing is invalid, and highlight the culprits.
 * I imagine this will require rejigging the drawing validation object to contain information on the culprit(s) locations.
 * Will need to use my skills gained from doing the hover things to decide how to adjust the view to show this, thinking some text,and then red borders to indicate problem cells.
 */

/* TODO_HIGH: Allow click and drag to paint in the drawing panel, of course I would want that!
 */

 /* TODO_MID: Allow hold shift and draw in order to draw in the 'other' color from what is selected in the colorpicker button.
  */