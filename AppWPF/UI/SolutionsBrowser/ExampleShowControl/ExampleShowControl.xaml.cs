using API;
using System.Windows;
using System.Windows.Controls;

namespace AppWPF;
/// <summary>
/// Interaction logic for ExampleShowControl.xaml
/// </summary>
public partial class ExampleShowControl : UserControl
{
    public ExampleShowControl()
    {
        InitializeComponent();
    }

    public IEnumerable<string> Words
    {
        get => (IEnumerable<string>)GetValue(WordsProperty);
        set => SetValue(WordsProperty, value);
    }

    public static readonly DependencyProperty WordsProperty =
        DependencyProperty.Register(
            nameof(Words),
            typeof(IEnumerable<string>),
            typeof(ExampleShowControl),
            new PropertyMetadata(null));

    public IEnumerable<ColorBoxVM> Colors
    {
        get => (IEnumerable<ColorBoxVM>)GetValue(ColorsProperty);
        set => SetValue(ColorsProperty, value);
    }

    public static readonly DependencyProperty ColorsProperty =
        DependencyProperty.Register(
            nameof(Colors),
            typeof(IEnumerable<ColorBoxVM>),
            typeof(ExampleShowControl),
            new PropertyMetadata(null));

    public string ExampleName
    {
        get => (string)GetValue(ExampleNameProperty);
        set => SetValue(ExampleNameProperty, value);
    }

    public static readonly DependencyProperty ExampleNameProperty =
        DependencyProperty.Register(
            nameof(ExampleName),
            typeof(string),
            typeof(ExampleShowControl),
            new PropertyMetadata(string.Empty));
}
