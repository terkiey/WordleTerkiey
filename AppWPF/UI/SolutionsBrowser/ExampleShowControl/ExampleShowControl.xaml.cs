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

    public static readonly DependencyProperty ExampleNameProperty =
        DependencyProperty.Register(
            nameof(ExampleName),
            typeof(string),
            typeof(ExampleShowControl),
            new PropertyMetadata(string.Empty));

    public string ExampleName
    {
        get => (string)GetValue(ExampleNameProperty);
        set => SetValue(ExampleNameProperty, value);
    }

    public static readonly DependencyProperty SolutionExampleVMProperty =
        DependencyProperty.Register(
            nameof(SolutionExampleVM),
            typeof(SolutionExampleVM),
            typeof(ExampleShowControl),
            new PropertyMetadata(null));

    public SolutionExampleVM SolutionExampleVM
    {
        get => (SolutionExampleVM)GetValue(SolutionExampleVMProperty);
        set => SetValue(SolutionExampleVMProperty, value);
    }

    
}
