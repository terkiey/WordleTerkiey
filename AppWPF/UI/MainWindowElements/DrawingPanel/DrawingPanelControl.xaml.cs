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

    private DrawingGridCell? _lastCellPainted;
    private DrawingGridCell? _lastCellHovered;

    public event EventHandler? UserDrawingDragCompleted;

    public DrawingPanelControl()
    {
        InitializeComponent();
    }

    private void ItemsControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isLeftDrag = true;
        _lastCellPainted = null; // reset last painted cell

        var itemsControl = sender as UIElement;
        itemsControl?.CaptureMouse();

        _ = ActivateItemUnderMouseAsync();
    }

    private void ItemsControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isRightDrag = true;
        _lastCellPainted = null;

        // Capture the mouse to the ItemsControl
        var itemsControl = sender as UIElement;
        itemsControl?.CaptureMouse();

        _ = ActivateItemUnderMouseAsync();
    }

    private void DrawingPanelControl_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        // Process what to do if the mouse is outside the cell grid.
        var pos = e.GetPosition(GridItemsControl);
        if (pos.X < 0 || pos.Y < 0 || pos.X > GridItemsControl.ActualWidth || pos.Y > GridItemsControl.ActualHeight)
        {
            if (_lastCellHovered != null)
            {
                _lastCellHovered.IsHighlighted = false;
                _lastCellHovered = null;
            }
        }

        // Process what to do if the mouse is over a cell.
        else if (TryGetCellUnderMouse(out DrawingGridCell? cell))
        {
            if (_isLeftDrag || _isRightDrag)
            {
                _ = PaintCellAsync(cell!);
            }

            // Hovering a new cell.
            if (_lastCellHovered != cell!)
            {
                if (_lastCellHovered != null)
                {
                    _lastCellHovered.IsHighlighted = false;
                }
                cell!.IsHighlighted = true;
                _lastCellHovered = cell;
            }
        }
    }

    private bool TryGetCellUnderMouse(out DrawingGridCell? cellUnderMouse)
    {
        cellUnderMouse = null;
        var itemsControl = this.GridItemsControl;

        // Get mouse position relative to the ItemsControl
        var pos = Mouse.GetPosition(itemsControl);

        // Perform hit test
        var hit = VisualTreeHelper.HitTest(itemsControl, pos);
        if (hit == null) return false;

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
                cellUnderMouse = cell;
                return true;
            }
        }
        return false;
    }

    private void ItemsControl_PreviewLeftMouseButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isLeftDrag = false;
        _lastCellPainted = null;

        var itemsControl = sender as UIElement;
        itemsControl?.ReleaseMouseCapture();

        UserDrawingDragCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void ItemsControl_PreviewRightMouseButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isRightDrag = false;
        _lastCellPainted = null;

        var itemsControl = sender as UIElement;
        itemsControl?.ReleaseMouseCapture();

        UserDrawingDragCompleted?.Invoke(this, EventArgs.Empty);
    }

    private async Task PaintCellAsync(DrawingGridCell cell)
    {
        var rootVm = (DrawingPanelViewModel)DataContext;

        if (_isLeftDrag)
            await CallColorBoxInCommand((DrawingGridCell)cell, rootVm);
        else if (_isRightDrag)
            await rootVm.ClearBoxCommand.ExecuteAsync((DrawingGridCell)cell);

        _lastCellPainted = cell;
    }

    private async Task ActivateItemUnderMouseAsync()
    {
        if (TryGetCellUnderMouse(out DrawingGridCell? cell))
        {
            // Dont paint if same cell as last painted.
            if (ReferenceEquals(_lastCellPainted, cell))
                return;

            await PaintCellAsync(cell!);
        }
    }

    private static async Task CallColorBoxInCommand(DrawingGridCell cell, DrawingPanelViewModel VM)
    {
        SyncShiftState(VM);
        await VM.ColorBoxInCommand.ExecuteAsync(cell);
    }

    private static void SyncShiftState(DrawingPanelViewModel VM)
    {
        bool actualShiftState = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        if (VM.IsShiftHeld != actualShiftState)
        {
            VM.IsShiftHeld = actualShiftState;
        }
    }
}