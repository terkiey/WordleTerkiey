using API;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AppWPF;

public class SolutionsBrowserViewModel : ISolutionsBrowserViewModel
{
    private readonly IWordleEngine _engine;
    private readonly ISolutionToExampleMapper _mapper;

    public ObservableCollection<Solution> CurrentSolutions { get; set; } = [];

    // Examples for display
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

    // Full solutions
    private CategorySolutionResult _exactSolutions = new(SolutionType.Exact, []);
    private CategorySolutionResult _shapeSolutions = new(SolutionType.Shape, []);
    private CategorySolutionResult _mirrorPaletteSolutions = new(SolutionType.MirrorPalette, []);

    public CategorySolutionResult ExactSolutions
    {
        get { return _exactSolutions; }
        private set
        {
            _exactSolutions = value;
            PropertyChanged?.Invoke(this, new(nameof(ExactSolutions)));
        }
    }

    public CategorySolutionResult ShapeSolutions
    {
        get { return _shapeSolutions; }
        private set
        {
            _shapeSolutions = value;
            PropertyChanged?.Invoke(this, new(nameof(ShapeSolutions)));
        }
    }

    public CategorySolutionResult MirrorPaletteSolutions
    {
        get { return _mirrorPaletteSolutions; }
        private set
        {
            _mirrorPaletteSolutions = value;
            PropertyChanged?.Invoke(this, new(nameof(MirrorPaletteSolutions)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public CommunityToolkit.Mvvm.Input.RelayCommand<string> OpenSolutionCategoryCommand { get; }


    public SolutionsBrowserViewModel(IWordleEngine engine, ISolutionToExampleMapper mapper)
    {
        _engine = engine;
        _engine.SolutionsReady += SolutionsReadyHandler;

        _mapper = mapper;

        OpenSolutionCategoryCommand = new (OpenSolutionCategoryCommandHandler);
    }

    private void SolutionsReadyHandler(object? sender, DrawingSolutionDTO DTO)
    {
        if (DTO.drawingValidation != DrawingValidation.Valid)
        {
            throw new InvalidOperationException("Drawing was invalid, solution should not have been requested!");
        }

        ClearExamples();
        UpdateExamples(DTO);

        ClearSolutions();
        UpdateSolutions(DTO);
    }

    private void ClearSolutions()
    {
        ExactSolutions = new(SolutionType.Exact, []);
        ShapeSolutions = new(SolutionType.Shape, []);
        MirrorPaletteSolutions = new(SolutionType.MirrorPalette, []);
    }

    private void UpdateSolutions(DrawingSolutionDTO DTO)
    {
        foreach (CategorySolutionResult categorySolution in DTO.categorySolutions)
        {
            if (categorySolution.solutions.Count == 0)
            {
                continue;
            }

            switch (categorySolution.category)
            {
                case SolutionType.Exact:
                    ExactSolutions = categorySolution;
                    break;

                case SolutionType.Shape:
                    ShapeSolutions = categorySolution;
                    break;

                case SolutionType.MirrorPalette:
                    MirrorPaletteSolutions = categorySolution;
                    break;

                default:
                    throw new ArgumentException("Solution type unspecified.");
            }
        }
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

            // Check the example has words (should do really)
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

    // Command Handlers
    private void OpenSolutionCategoryCommandHandler(string? categoryString)
    {
        if (categoryString is null)
        {
            return;
        }

        switch (categoryString.ToLower())
        {
            case "exact":
                CurrentSolutions = [.. ExactSolutions.solutions];
                break;

            case "shape":
                CurrentSolutions = [.. ShapeSolutions.solutions];
                break;

            case "mirrorpalette":
                CurrentSolutions = [.. MirrorPaletteSolutions.solutions];
                break;

            default:
                throw new ArgumentException();
        }

        var solutionWindow = new SolutionsWindow()
        {
            DataContext = this,
        };
        PropertyChanged?.Invoke(this, new(nameof(CurrentSolutions)));
        solutionWindow.ShowDialog();
    }

    /* TODO_MID: Allow the user to click the panel for a type of solution, which opens a window, or adjusts the solutionbrowser panel, whatever, it will have these features:
     * Show all the unique available patterns.
     * Allow, for each row in a selected pattern diagram, to see the possible allowed words list (and to search that list)
     * 
     * Then, clicking a pattern opens a further window (or navigates deeper into the panel), allowing the user to browse all of the possible words that give each row of the solution.
     */

    /* TODO_MID: Add a feature so that the user can save drawings, load drawings(from file, not in memory) and also to select drawings(include a select all button) and solve for all of the selected drawings
     * 
     * Ideally it would indicate which solutions are available for each drawing (so a bar separated into 3 sections, green if each solution type is available, red if not)
     * Assuming the solutions are their 'picture' like the example but without words. Alongside this available solution bar.
     * 
     * If a user double-clicks a solution, or right-click and presses (open), or selects a single solution and clicks the 'load' button, it fills the drawing grid with that drawing and presses solve for them
     */
}
