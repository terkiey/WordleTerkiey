using System.Text.RegularExpressions;

namespace API;

/// <summary>
/// Attempts to use regex to find solutions, though its not complete, and would require breaking words into letters to account for edge-cases.
/// </summary>
internal class DrawingSolver : IDrawingSolver
{
    private readonly IWordleDictionary _wordleDictionary;
    public WordleWord AnswerWord => _wordleDictionary.AnswerWord;
    public HashSet<WordleWord> GuessableWords => _wordleDictionary.AllowedWords;
    private Dictionary<WordClue, HashSet<WordleWord>> CachedSolutions = [];

    public DrawingSolver(IWordleDictionary wordleDictionary)
    {
        _wordleDictionary = wordleDictionary;
        _wordleDictionary.AnswerWordChanged += AnswerChangedHandler;
    }

    public DrawingSolutionDTO Solve(BoardClue userDrawing)
    {
        List<CategorySolutionResult> categorySolutions = [];
        DrawingValidation validate = ValidateDrawing(userDrawing);
        if  (validate != DrawingValidation.Valid)
        {
            return new(validate, categorySolutions);
        }

        List<Solution> _solutionList = [];
        CategorySolutionResult _categorySolution;

        // Record exact solution.
        Solution exactSolution = ExactSolve(userDrawing);
        _solutionList.Add(exactSolution);
        _categorySolution = new(SolutionType.Exact, _solutionList.ToList());
        categorySolutions.Add(_categorySolution);
        _solutionList.Clear();

        // Record shape solution.
        /*
        _solutionList = ShapeSolve(userDrawing);
        _categorySolution = new(SolutionType.Shape, _solutionList);
        categorySolutions.Add(_categorySolution);
        _solutionList.Clear(); */

        // record MissOne solution.
        /*
        _solutionList = MissOneSolve(userDrawing);
        _categorySolution = new(SolutionType.MissOne, _solutionList);
        categorySolutions.Add(_categorySolution);
        _solutionList.Clear(); */

        return new(validate, categorySolutions);
    }

    public DrawingValidation ValidateDrawing(BoardClue userDrawing)
    {
        WordClue answer = new();
        WordClue blank = new();
        for (int i = 0; i < 5; i++)
        {
            answer[i] = BoxColor.Green;
        }

        // If answer row is given, then the rest of drawing must be blank.
        // Impossible to have a row with 4 green and 1 yellow.
        for (int rowIndex = 0; rowIndex < 6; rowIndex++)
        {
            WordClue row = userDrawing[rowIndex];
            if (row.CountGreen() == 4 && row.CountYellow() == 1)
            {
                return DrawingValidation.ImpossibleRow;
            }

            if (row == answer)
            {
                for (int laterRowIndex = rowIndex + 1; laterRowIndex < 6; laterRowIndex++)
                {
                    WordClue futureRow = userDrawing[laterRowIndex];
                    if (futureRow != blank)
                    {
                        return DrawingValidation.EarlyAnswer;
                    }
                }
            }
        }

        return DrawingValidation.Valid;
    }

    // TODO_LOW: This will need to create a list of rows it wants to make, make them, and then construct solutions from the combinations of row solutions.
    private List<Solution> MissOneSolve(BoardClue userDrawing)
    {
        throw new NotImplementedException();
    }

    // TODO_LOW: This will need to create a list of rows it wants to make, make them, and then construct solutions from the combinations of row solutions.
    private List<Solution> ShapeSolve(BoardClue userDrawing)
    {
        throw new NotImplementedException();
        // Create all possible boards that have the same colored shape, but with some greens replaced with yellow.
        for (int wordIndex = 0; wordIndex < 6; wordIndex++)
        {
            WordClue wordDrawing = userDrawing[wordIndex];
            for (int letterIndex = 0; letterIndex < 5; letterIndex++)
            {
                BoxColor letterDrawing = wordDrawing[letterIndex];
                if (letterDrawing == BoxColor.Green)
                {
                    
                }
            }
        }

    }

    // TODO_LOW: Make this re-use solutions if multiple rows have same value.
    private Solution ExactSolve(BoardClue userDrawing)
    {
        string[] regexConditions = new string[6];
        SolutionWords solutionWords = new();
        
        // Loop over rows (word drawings).
        for (int rowIndex = 0; rowIndex < 6; rowIndex++)
        {  
            WordClue rowDrawing = userDrawing[rowIndex];
            // Check cache for the rowDrawing and just use that as the solution words if present.
            HashSet<WordleWord>? cachedWords;
            if (CachedSolutions.TryGetValue(rowDrawing, out cachedWords))
            {
                solutionWords[rowIndex] = cachedWords;
                continue;
            }

            // Create regex conditions for check on guessable words
            HashSet<char> greenedOut = GreenedOut(rowDrawing);

            string regexInternal = "";
            for (int letterIndex = 0; letterIndex < 5; letterIndex++)
            {
                regexInternal += RegexMaker(letterIndex, greenedOut, rowDrawing);
            }

            regexConditions[rowIndex] = "^" + regexInternal + "$";

            // Check guessable words for words that match conditions, and add result to cache.
            string condition = regexConditions[rowIndex];
            solutionWords[rowIndex] = GuessableWords
                                                .Where(s => Regex.IsMatch(s.ToString(), condition))
                                                .ToHashSet();
            CachedSolutions.Add(rowDrawing, solutionWords[rowIndex]);
        }

        return new Solution(userDrawing, solutionWords);
    }

    private string GreenRegex(int letterIndex)
    {
        return AnswerWord[letterIndex].ToString();
    }

    private string YellowRegex(int letterIndex, HashSet<char> greenedOut)
    {
        string answerString = AnswerWord.ToString();
        var yellowLetters = answerString.Distinct<char>()
                                                .Where(character => (character != answerString[letterIndex]))
                                                .Except(greenedOut);
        string yellowLetterString = new(yellowLetters.ToArray());
        return "[" + yellowLetterString + "]"; 
    }

    private string BlackRegex(int letterIndex, HashSet<char> greenedOut)
    {
        string answerString = AnswerWord.ToString();
        string greenLetter = AnswerWord[letterIndex].ToString();
        var yellowLetters = answerString.Distinct<char>()
                                                .Where(character => (character != answerString[letterIndex]))
                                                .Except(greenedOut);
        string yellowLetterString = new(yellowLetters.ToArray());
        string disallowedLetters = greenLetter + yellowLetterString;
        return "[^" + disallowedLetters + "]";
    }

    private string RegexMaker(int letterIndex, HashSet<char> greenedOut, WordClue wordClue)
    {
        BoxColor lettercolor = wordClue[letterIndex];
        switch (lettercolor)
        {
            case BoxColor.Green:
                return GreenRegex(letterIndex);
            case BoxColor.Yellow:
                return YellowRegex(letterIndex, greenedOut);
            default:
                return BlackRegex(letterIndex, greenedOut);
        }
    }

    // TODO_HIGH: LETTERS CAN BE YELLOWED OUT TOO!!!! INCORPORATE INTO THIS FUNCTION!
    private HashSet<char> GreenedOut(WordClue wordDrawing)
    {
        Dictionary<char, int> countCharsAll = [];
        Dictionary<char, int> countCharsGreen = [];
        string AnswerString = AnswerWord.ToString();
        for (int letterIndex = 0; letterIndex < 5; letterIndex++)
        {
            // 1. Count how many times each letter appears.
            char answerLetter = AnswerString[letterIndex];
            if (countCharsAll.TryGetValue(answerLetter, out _))
            {
                countCharsAll[answerLetter] += 1;
            }
            else
            {
                countCharsAll.Add(answerLetter, 1);
            }

            // 2 Count how many times each letter is greened.
            if (wordDrawing[letterIndex] == BoxColor.Green)
            {
                if (countCharsGreen.TryGetValue(answerLetter, out _))
                {
                    countCharsGreen[answerLetter] += 1;
                }
                else
                {
                    countCharsGreen.Add(answerLetter, 1);
                }
            }
        }

        // 3. If letter is green always, then it is 'greened out'.
        HashSet<char> greenedOut = [];
        foreach (char answerLetter in countCharsAll.Keys)
        {
            if (!countCharsGreen.TryGetValue(answerLetter, out int greenCount))
            {
                continue;
            }

            if (countCharsAll[answerLetter] == greenCount)
            {
                greenedOut.Add(answerLetter);
            }
        }

        return greenedOut;
    }

    // Event Handlers
    private void AnswerChangedHandler(object? sender, WordleWord answerWord)
    {
        CachedSolutions.Clear();
    }
}
