using API;
using System.ComponentModel;

namespace AppWPF;

public class SolutionsBrowserViewModel : ISolutionsBrowserViewModel
{
    private readonly IWordleEngine _engine;
    private readonly ISolutionToExampleMapper _mapper;

    private SolutionExampleVM _exactSolutionExample = new();
    private SolutionExampleVM _shapeSolutionExample = new();
    private SolutionExampleVM _mirrorPaletteSolutionExample = new();

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

    public SolutionExampleVM MirrorPaletteSolutionExample
    {
        get { return _mirrorPaletteSolutionExample; }
        private set
        {
            _mirrorPaletteSolutionExample = value;
            PropertyChanged?.Invoke(this, new(nameof(MirrorPaletteSolutionExample)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public SolutionsBrowserViewModel(IWordleEngine engine, ISolutionToExampleMapper mapper)
    {
        _engine = engine;
        _engine.SolutionsReady += SolutionsReadyHandler;

        _mapper = mapper;
    }

    private void SolutionsReadyHandler(object? sender, DrawingSolutionDTO DTO)
    {
        if (DTO.drawingValidation != DrawingValidation.Valid)
        {
            throw new InvalidOperationException("Drawing was invalid, solution should not have been requested!");
        }

        ClearExamples();
        UpdateExamples(DTO);
    }

    private void UpdateExamples(DrawingSolutionDTO DTO)
    {
        bool exactExampleUpdated = false;
        bool shapeExampleUpdated = false;
        bool mirrorPaletteExampleUpdated = false;
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

            if (categorySolution.category == SolutionType.MirrorPalette && mirrorPaletteExampleUpdated == false)
            {
                UpdateExample(SolutionType.MirrorPalette, categorySolution.solutions[0]);
                mirrorPaletteExampleUpdated = true;
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

            case SolutionType.MirrorPalette:
                MirrorPaletteSolutionExample = example;
                PropertyChanged?.Invoke(this, new(nameof(MirrorPaletteSolutionExample)));
                break;
        }
    }

    private void ClearExamples()
    {
        ExactSolutionExample = new();
        ShapeSolutionExample = new();
        MirrorPaletteSolutionExample = new();
    }
    /* TODO_MID: Allow the user to click the panel for a type of solution, which opens a window, or adjusts the solutionbrowser pannel, whatever, it will have these features:
     * Show all the unique available patterns.
     * Allow, for each row in a selected pattern diagram, to see the possible allowed words list (and to search that list)
     * 
     * Then, clicking a pattern opens a further window (or navigates deeper into the panel), allowing the user to browse all of the possible words that give each row of the solution.
     */

    /* TODO_MID: This should conditionally show the user control to show the a category example (put those into user controls btw),
    * when no example is available, it should indicate as such with a different user control that it swaps in place to show that class of solution is impossible.
    */
}
