using API;
using System.ComponentModel;

namespace AppWPF;

public class SolutionsBrowserViewModel : ISolutionsBrowserViewModel
{
    private readonly IWordleEngine _engine;
    private readonly ISolutionToExampleMapper _mapper;

    public SolutionExampleVM exactSolutionExample { get; private set; }
    public SolutionExampleVM shapeSolutionExample { get; private set; }
    public SolutionExampleVM missOneSolutionExample { get; private set; } 

    public event PropertyChangedEventHandler? PropertyChanged;

    public SolutionsBrowserViewModel(IWordleEngine engine, ISolutionToExampleMapper mapper)
    {
        _engine = engine;
        _engine.SolutionsReady += SolutionsReadyHandler;

        _mapper = mapper;

        exactSolutionExample = new();
        shapeSolutionExample = new();
        missOneSolutionExample = new();
    }

    // TODO_MID: This should clear the solutions prior to setting new ones, so that old solutions dont confusingly stay.
    private void SolutionsReadyHandler(object? sender, DrawingSolutionDTO DTO)
    {
        if (DTO.drawingValidation != DrawingValidation.Valid)
        {
            throw new ArgumentOutOfRangeException(nameof(DTO.drawingValidation), "Drawing was invalid, solution should not have been requested!");
        }

        bool exactExampleUpdated = false;
        bool shapeExampleUpdated = false;
        bool missOneExampleUpdated = false;
        foreach (CategorySolutionResult categorySolution in DTO.categorySolutions)
        {
            if (categorySolution.solutions.Count == 0)
            {
                continue;
            }

            bool failed = false;
            for (int wordIndex = 0; wordIndex < 5; wordIndex++)
            {
                if (categorySolution.solutions[0].words[wordIndex].Count == 0)
                {
                    failed = true;
                }
            }  
            if (failed)
            {
                continue;
            }

            if (categorySolution.category == SolutionType.Exact && exactExampleUpdated == false)
            {
                UpdateExample(SolutionType.Exact, categorySolution.solutions[0]);
                exactExampleUpdated = true;
            }

            if(categorySolution.category == SolutionType.Shape && shapeExampleUpdated == false)
            {
                UpdateExample(SolutionType.Shape, categorySolution.solutions[0]);
                shapeExampleUpdated = true;
            }

            if(categorySolution.category == SolutionType.MissOne && missOneExampleUpdated == false)
            {
                UpdateExample(SolutionType.MissOne, categorySolution.solutions[0]);
                missOneExampleUpdated = true;
            }
        }
    }

    private void UpdateExample(SolutionType type, Solution solution)
    {
        SolutionExampleVM example = _mapper.MapSolutionToExample(solution);
        switch (type)
        {
            case SolutionType.Exact:
                exactSolutionExample = example;
                PropertyChanged?.Invoke(this, new(nameof(exactSolutionExample)));
                break;

            case SolutionType.Shape:
                shapeSolutionExample = example;
                PropertyChanged?.Invoke(this, new(nameof(shapeSolutionExample)));
                break;

            case SolutionType.MissOne:
                missOneSolutionExample = example;
                PropertyChanged?.Invoke(this, new(nameof(missOneSolutionExample)));
                break;
        }
    }
    /* TODO_HIGH: Turn each solution-type panel into a user-control to reduce code verbosity and allow me to uniformly make design changes to them without having to make
     * the changes one time for each panel.
     */

    /* TODO_MID: Allow the user to click the panel for a type of solution, which opens a window, or adjusts the solutionbrowser pannel, whatever, it will have these features:
     * Show all the available patterns.
     * 
     * Then, clicking a pattern opens a further window (or navigates deeper into the panel), allowing the user to browse all of the possible words that give each row of the solution.
     */
}
