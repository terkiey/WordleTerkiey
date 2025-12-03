using Microsoft.Extensions.DependencyInjection;

namespace API;

public static class API_DI
{
    public static IServiceCollection AddAPI(this IServiceCollection services)
    {
        services.AddSingleton<IGuessableWordService, GuessableWordService>(_ => new GuessableWordService());
        services.AddSingleton<IAnswerWordService, AnswerWordService>(_ => new AnswerWordService());
        services.AddSingleton<IBoardMapper, BoardMapper>(_ => new BoardMapper());

        services.AddSingleton<IWordleDictionary, WordleDictionary>(sp =>
        {
            var guessableWordService = sp.GetRequiredService<IGuessableWordService>();
            var answerWordService = sp.GetRequiredService<IAnswerWordService>();
            return new WordleDictionary(guessableWordService, answerWordService);
        });

        services.AddSingleton<IDrawingSolver, DrawingSolverBrute>(sp =>
        {   
            var wordleDictionary = sp.GetRequiredService<IWordleDictionary>();
            var boardMapper = sp.GetRequiredService<IBoardMapper>();
            return new DrawingSolverBrute(wordleDictionary, boardMapper);
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
