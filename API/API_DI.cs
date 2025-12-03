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
        });

        services.AddSingleton<IDrawingSolver, DrawingSolver>(sp =>
        {   
            var wordleDictionary = sp.GetRequiredService<IWordleDictionary>();
            return new DrawingSolver(wordleDictionary);
        });

        services.AddSingleton<IWordleEngine, WordleEngine>(sp =>
        {
            var drawingSolver = sp.GetRequiredService<IDrawingSolver>();
            var wordleDictionary = sp.GetRequiredService<IWordleDictionary>();
            return new WordleEngine(drawingSolver, wordleDictionary);
        });

        return services;
    }
}
