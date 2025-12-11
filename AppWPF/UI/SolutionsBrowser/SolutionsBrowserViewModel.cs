using API;
using System.ComponentModel;

namespace AppWPF;

public class SolutionsBrowserViewModel : ISolutionsBrowserViewModel
{
    private readonly IWordleEngine _engine;
    private readonly ISolutionToExampleMapper _mapper;

    private SolutionExampleVM _exactSolutionExample;
    private SolutionExampleVM _shapeSolutionExample;
    private SolutionExampleVM _missOneSolutionExample;

    public SolutionExampleVM ExactSolutionExample
    {
        get { return _exactSolutionExample; }
        private set
        {
            _exactSolutionExample = value;
            PropertyChanged?.Invoke(this, new(nameof(ExactSolutionExample)));
        }
    }

    public SolutionExampleVM ShapeSolutionExample
    {
        get { return _shapeSolutionExample; }
        private set
        {
            _shapeSolutionExample = value;
            PropertyChanged?.Invoke(this, new(nameof(ShapeSolutionExample)));
        }
    }

    public SolutionExampleVM MissOneSolutionExample
    {
        get { return _missOneSolutionExample; }
        private set
        {
            _missOneSolutionExample = value;
            PropertyChanged?.Invoke(this, new(nameof(MissOneSolutionExample)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public SolutionsBrowserViewModel(IWordleEngine engine, ISolutionToExampleMapper mapper)
    {
        _engine = engine;
        _engine.SolutionsReady += SolutionsReadyHandler;

        _mapper = mapper;

        ExactSolutionExample = new();
        ShapeSolutionExample = new();
        MissOneSolutionExample = new();
    }

    private void SolutionsReadyHandler(object? sender, DrawingSolutionDTO DTO)
    {
        if (DTO.drawingValidation != DrawingValidation.Valid)
        {
            throw new ArgumentOutOfRangeException(nameof(DTO.drawingValidation), "Drawing was invalid, solution should not have been requested!");
        }

        ClearExamples();
        UpdateExamples(DTO);
    }

    private void UpdateExamples(DrawingSolutionDTO DTO)
    {
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
            for (int wordIndex = 0; wordIndex < 6; wordIndex++)
            {
                if (categorySolution.solutions[0].words[wordIndex].Count == 0)
                {
                    failed = true;
                    break;
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

            if (categorySolution.category == SolutionType.Shape && shapeExampleUpdated == false)
            {
                UpdateExample(SolutionType.Shape, categorySolution.solutions[0]);
                shapeExampleUpdated = true;
            }

            if (categorySolution.category == SolutionType.MissOne && missOneExampleUpdated == false)
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
                ExactSolutionExample = example;
                PropertyChanged?.Invoke(this, new(nameof(ExactSolutionExample)));
                break;

            case SolutionType.Shape:
                ShapeSolutionExample = example;
                PropertyChanged?.Invoke(this, new(nameof(ShapeSolutionExample)));
                break;

            case SolutionType.MissOne:
                MissOneSolutionExample = example;
                PropertyChanged?.Invoke(this, new(nameof(MissOneSolutionExample)));
                break;
        }
    }

    private void ClearExamples()
    {
        ExactSolutionExample = new();
        ShapeSolutionExample = new();
        MissOneSolutionExample = new();
    }
    /* TODO_MID: Make the intense logic calls async so they dont block the UI out. Also add some UI element for when solving is in progress, maybe a small animation.

    /* TODO_HIGH: Turn each solution-type panel into a user-control to reduce code verbosity and allow me to uniformly make design changes to them without having to make
     * the changes one time for each panel.
     */

    /* TODO_MID: Allow the user to click the panel for a type of solution, which opens a window, or adjusts the solutionbrowser pannel, whatever, it will have these features:
     * Show all the available patterns.
     * 
     * Then, clicking a pattern opens a further window (or navigates deeper into the panel), allowing the user to browse all of the possible words that give each row of the solution.
     */

     /* TODO_HIGH: Replace MissOne solves with rotated solves (horizontal flip, and vertical flip, and both) */
}
