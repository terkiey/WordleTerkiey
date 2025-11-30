using Microsoft.Extensions.DependencyInjection;

namespace API;

public static class API_DI
{
    public static IServiceCollection AddAPI(this IServiceCollection services)
    {
        services.AddSingleton<IAnswerWordService, AnswerWordService>();
        services.AddSingleton<IGuessableWordService, GuessableWordService>();

        services.AddSingleton<IWordleDictionary, WordleDictionary>();

        return services;
    }
}
