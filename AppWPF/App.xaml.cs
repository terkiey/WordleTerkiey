using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace AppWPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // resolve dependencies (DI)
        var services = new ServiceCollection();

        services.AddUI();

        var provider = services.BuildServiceProvider();

        MainWindow mainWindow = provider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
