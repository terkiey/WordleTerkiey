using API;
using System.ComponentModel;

namespace AppWPF;

public class DrawingGridCell : INotifyPropertyChanged
{
    private BoxColor _color;
    public int Row { get; set; }
    public int Column { get; set; }
    public BoxColor Color { get => _color;
        set
        {
            _color = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public DrawingGridCell(int row, int column)
    {
        Row = row;
        Column = column;
        Color = new BoxColor();
    }

    private bool _isHighlighted = false;
    public bool IsHighlighted 
    { 
        get => _isHighlighted;
        set 
        { 
            _isHighlighted = value; 
            PropertyChanged?.Invoke(this, new(nameof(IsHighlighted)));
        }
    }
}
