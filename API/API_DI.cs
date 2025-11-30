using Microsoft.Extensions.DependencyInjection;

namespace API;

public static class API_DI
{
    public static IServiceCollection AddAPI(this IServiceCollection services)
    {
        services.AddSingleton<IGuessableWordService, GuessableWordService>(_ => new GuessableWordService());
        services.AddSingleton<IAnswerWordService, AnswerWordService>(_ => new AnswerWordService());

        services.AddSingleton<IWordleDictionary, WordleDictionary>(sp =>
        {
            var guessableWordService = sp.GetRequiredService<IGuessableWordService>();
            var answerWordService = sp.GetRequiredService<IAnswerWordService>();
            return new WordleDictionary(guessableWordService, answerWordService);
        }

        return services;
    }
}
