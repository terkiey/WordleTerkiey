using API;
using Microsoft.Extensions.DependencyInjection;

namespace AppWPF;

public static class WPF_DI
{
    public static IServiceCollection AddUI(this IServiceCollection services)
    {
        services.AddAPI();

        services.AddSingleton<ISidebarViewModel, SidebarViewModel>();
        services.AddSingleton<ISolutionsBrowserViewModel, SolutionsBrowserViewModel>();
        services.AddSingleton<IDrawingPanelViewModel, DrawingPanelViewModel>();

        services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        return services;
    }
}
