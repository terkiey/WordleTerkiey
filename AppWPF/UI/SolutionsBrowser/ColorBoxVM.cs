using API;

namespace AppWPF;

public class ColorBoxVM
{
    public BoxColor Color { get; set; }
    public ColorBoxVM(BoxColor color) => Color = color;
}