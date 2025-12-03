using System.Windows.Controls;

namespace AppWPF;
/// <summary>
/// Interaction logic for SidebarControl.xaml
/// </summary>
public partial class SidebarControl : UserControl
{
    public SidebarControl()
    {
        InitializeComponent();
    }
}

// TODO_HIGH: Adjust the button hover effects so that it's less intrusive (maybe a border grow and glow, or something like that).
// TODO_HIGH: Figure out how to make the picture buttons more visual with when they are and aren't allowed to be clicked, in fact, dont do my new hover effects in those cases too.