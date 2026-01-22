namespace API;

internal class DrawingSolverBrute : IDrawingSolver
{
    private readonly IWordleDictionary _wordleDictionary;
    private readonly IBoardMapper _boardMapper;

    private readonly Dictionary<WordClue, HashSet<WordleWord>> _cachedSolutions = [];
    private readonly Dictionary<WordleWord, WordClue> _cachedDrawings = [];

    public WordleWord AnswerWord => _wordleDictionary.AnswerWord;
    public HashSet<WordleWord> GuessableWords => _wordleDictionary.AllowedWords;
    
    public DrawingSolverBrute(IWordleDictionary wordleDictionary, IBoardMapper boardMapper)
    {
        _wordleDictionary = wordleDictionary;
        _wordleDictionary.AnswerWordChanged += AnswerChangedHandler;

        _boardMapper = boardMapper;
    }

    public DrawingSolutionDTO Solve(BoardClue userDrawing)
    {
        List<CategorySolutionResult> categorySolutions = [];
        DrawingValidation validate = ValidateDrawing(userDrawing);
        if (validate != DrawingValidation.Valid)
        {
            return new(validate, categorySolutions);
        }

        List<Solution> _solutionList = [];
        CategorySolutionResult _categorySolution;

        // Record exact solution.
        if (TryExactSolve(userDrawing, out Solution? boardSolution))
        {
            _solutionList.Add(boardSolution!);
        }
        _categorySolution = new(SolutionType.Exact, [.. _solutionList]);
        categorySolutions.Add(_categorySolution);
        _solutionList.Clear();

        // Record shape solution.
        List<BoardClue> shapeBoards = _boardMapper.MapToShape(userDrawing);
        foreach (BoardClue board in shapeBoards)
        {
            DrawingValidation validateBoard = ValidateDrawing(board);
            if (validateBoard != DrawingValidation.Valid) { continue; }
            if (TryExactSolve(board, out boardSolution))
            {
                _solutionList.Add(boardSolution!);
            }
        }
   
        _categorySolution = new(SolutionType.Shape, [.. _solutionList]);
        categorySolutions.Add(_categorySolution);
        _solutionList.Clear();

        // record Mirrored + Palette Swap Positions
        List<BoardClue> mirrorBoards = _boardMapper.MapToMirrors(userDrawing);

        List<BoardClue> mirroredPaletteBoards = [];
        foreach (BoardClue mirrorBoard in mirrorBoards)
        {
            mirroredPaletteBoards.Add(mirrorBoard);
            mirroredPaletteBoards.AddRange(_boardMapper.MapToShape(mirrorBoard));
        }

        foreach (BoardClue toSolve in mirroredPaletteBoards)
        {
            if (ValidateDrawing(toSolve) != DrawingValidation.Valid) { continue; }
            if (TryExactSolve(toSolve, out boardSolution))
            {
                _solutionList.Add(boardSolution!);
            }
        }

        _categorySolution = new(SolutionType.MirrorPalette, [.. _solutionList]);
        categorySolutions.Add(_categorySolution);
        _solutionList.Clear();

        return new(validate, categorySolutions);
    }

    private bool TryExactSolve(BoardClue userDrawing, out Solution? solution)
    {
        solution = null;
        SolutionWords solutionWords = new();

        // Loop over rows (word drawings).
        for (int rowIndex = 0; rowIndex < 6; rowIndex++)
        {
            WordClue rowDrawing = userDrawing[rowIndex];
            // Check cache for the rowDrawing and just use that as the solution words if present.
            if (_cachedSolutions.TryGetValue(rowDrawing, out HashSet<WordleWord>? cachedWords))
            {
                solutionWords[rowIndex] = cachedWords;
            }

            // Check every guessable word, and if matches the output drawing, add to solution list, (and cache it)
            else
            {
                foreach (WordleWord guess in GuessableWords)
                {
                    string test = guess.ToString().ToUpper();
                    WordClue testDraw = Draw(guess);
                    bool testComp = Draw(guess) == rowDrawing;
                    if (Draw(guess) == rowDrawing)
                    {
                        solutionWords[rowIndex].Add(guess);
                    }
                }
                _cachedSolutions.Add(rowDrawing, solutionWords[rowIndex]);
            }

            if (solutionWords[rowIndex].Count == 0)
            {
                return false;
            }
        }

        solution = new Solution(userDrawing, solutionWords);
        return true;
    }

    public DrawingValidation ValidateDrawing(BoardClue userDrawing)
    {
        WordClue answer = new();
        WordClue blank = new();
        for (int i = 0; i < 5; i++)
        {
            answer[i] = BoxColor.Green;
        }

        /* Rules:
         * If answer row is given, then the rest of drawing must be blank.
         * Impossible to have a row with 4 green and 1 yellow.
         */
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

    private WordClue Draw(WordleWord guess)
    {
        if (_cachedDrawings.TryGetValue(guess, out WordClue? cachedDrawing))
        {
            return cachedDrawing;
        }

        Dictionary<WordleLetter, int> answerLetterCountDict = [];
        for (int letterIndex = 0; letterIndex < 5; letterIndex++)
        {
            WordleLetter answerLetter = AnswerWord[letterIndex];
            // Count answer letters.
            answerLetterCountDict[answerLetter] = answerLetterCountDict.GetValueOrDefault(answerLetter) + 1;
        }

        WordClue drawnRow = new();
        Dictionary<WordleLetter, int> letterHitsDict = [];

        // Green Cells Loop
        for (int letterIndex = 0; letterIndex < 5; letterIndex++)
        {
            WordleLetter guessLetter = guess[letterIndex];
            WordleLetter answerLetter = AnswerWord[letterIndex];

            bool guessPresentInHits = letterHitsDict.TryGetValue(guessLetter, out _);

            if (guessLetter == answerLetter)
            {
                drawnRow[letterIndex] = BoxColor.Green;
                if (guessPresentInHits)
                {
                    letterHitsDict[guessLetter]++;
                }
                else
                {
                    letterHitsDict.Add(guessLetter, 1);
                }
            }
        }

        // Yellow cells Loop
        for (int letterIndex = 0; letterIndex < 5; letterIndex++)
        {
            WordleLetter guessLetter = guess[letterIndex];
            bool guessPresentInHits = letterHitsDict.TryGetValue(guessLetter, out int letterHits);
            bool guessPresentInAnswer = answerLetterCountDict.TryGetValue(guessLetter, out int answerLetterCount);
            if (guessPresentInAnswer && drawnRow[letterIndex] != BoxColor.Green)
            {
                if (!guessPresentInHits || letterHits < answerLetterCount)
                {
                    drawnRow[letterIndex] = BoxColor.Yellow;
                    if (!guessPresentInHits)
                    {
                        letterHitsDict.Add(guessLetter, 1);
                    }
                    else
                    {
                        letterHitsDict[guessLetter]++;
                    }
                }
            }
        }

        // Otherwise the color defaults to black.

        _cachedDrawings.Add(guess, drawnRow);
        return drawnRow;
    }

    // Event Handlers
    private void AnswerChangedHandler(object? sender, WordleWord answerWord)
    {
        _cachedSolutions.Clear();
        _cachedDrawings.Clear();
    }
}
