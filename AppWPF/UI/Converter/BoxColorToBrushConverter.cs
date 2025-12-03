using API;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AppWPF;

public class BoxColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BoxColor boxColor)
        {
            return boxColor switch
            {
                BoxColor.Black => Application.Current.Resources["WordleBlackBrush"],
                BoxColor.Yellow => Application.Current.Resources["WordleYellowBrush"],
                BoxColor.Green => Application.Current.Resources["WordleGreenBrush"],
                _ => Brushes.HotPink
            };
        }

        return Brushes.HotPink;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
