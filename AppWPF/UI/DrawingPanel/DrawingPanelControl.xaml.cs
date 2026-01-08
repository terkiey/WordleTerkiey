using System.Diagnostics;
using System.Formats.Asn1;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AppWPF;
/// <summary>
/// Interaction logic for DrawingPanelControl.xaml
/// </summary>
public partial class DrawingPanelControl : UserControl
{
    private bool _isLeftDrag;
    private bool _isRightDrag;

    private object? _lastCell;

    public event EventHandler? UserDrawingDragCompleted;

    public DrawingPanelControl()
    {
        InitializeComponent();
    }

    private void ItemsControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isLeftDrag = true;
        _lastCell = null; // reset last painted cell

        var itemsControl = sender as UIElement;
        itemsControl?.CaptureMouse();

        _ = ActivateItemUnderMouseAsync();
    }

    private void ItemsControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isRightDrag = true;
        _lastCell = null;

        // Capture the mouse to the ItemsControl
        var itemsControl = sender as UIElement;
        itemsControl?.CaptureMouse();

        _ = ActivateItemUnderMouseAsync();
    }

    private void ItemsControl_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (_isLeftDrag || _isRightDrag)
        {
            _ = ActivateItemUnderMouseAsync();
        }
    }

    private void ItemsControl_PreviewLeftMouseButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isLeftDrag = false;
        _lastCell = null;

        var itemsControl = sender as UIElement;
        itemsControl?.ReleaseMouseCapture();

        UserDrawingDragCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void ItemsControl_PreviewRightMouseButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isRightDrag = false;
        _lastCell = null;

        var itemsControl = sender as UIElement;
        itemsControl?.ReleaseMouseCapture();

        UserDrawingDragCompleted?.Invoke(this, EventArgs.Empty);
    }

    private async Task ActivateItemUnderMouseAsync()
    {
        var itemsControl = this.GridItemsControl;

        // Get mouse position relative to the ItemsControl
        var pos = Mouse.GetPosition(itemsControl);

        // Perform hit test
        var hit = VisualTreeHelper.HitTest(itemsControl, pos);
        if (hit == null) return;

        // Walk up visual tree until we find a Border with a DataContext
        var dep = hit.VisualHit;
        while (dep != null && !(dep is Border border && border.DataContext != null))
        {
            dep = VisualTreeHelper.GetParent(dep);
        }

        if (dep is Border foundBorder && foundBorder.DataContext != null)
        {
            var cellVm = foundBorder.DataContext;

            if (cellVm is DrawingGridCell cell)
            {

                if (ReferenceEquals(_lastCell, cell))
                    return;

                _lastCell = cell;

                var rootVm = (DrawingPanelViewModel)DataContext;

                if (_isLeftDrag)
                    await rootVm.ColorBoxInCommand.ExecuteAsync((DrawingGridCell)cell);
                else if (_isRightDrag)
                    await rootVm.ClearBoxCommand.ExecuteAsync((DrawingGridCell)cell);
            }
        }
    }
}   

 /* TODO_MID: Allow hold shift and draw in order to draw in the 'other' color from what is selected in the colorpicker button.
  */