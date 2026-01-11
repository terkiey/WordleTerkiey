using API;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace AppWPF;

public class SolutionBoardConverter : IValueConverter
{
    // Ripped this straight from the mapper, probably stupid to have them separate, maybe should put this method in the mapper...
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BoardClue board)
        {
            ObservableCollection<BoxColor> colors = [];
           
            // iterate over rows to get the colors from solution.
            for (int rowIndex = 0; rowIndex < 6; rowIndex++)
            {
                for (int letterIndex = 0; letterIndex < 5; letterIndex++)
                {
                    BoxColor color = board[rowIndex][letterIndex];
                    colors.Add(color);
                }
            }
            return colors;
        }

        return new ObservableCollection<BoxColor>();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
