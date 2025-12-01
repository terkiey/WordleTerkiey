using System.Text.RegularExpressions;

namespace API;

internal class DrawingSolver : IDrawingSolver
{
    private readonly IWordleDictionary _wordleDictionary;
    public WordleWord AnswerWord => _wordleDictionary.AnswerWord;
    public HashSet<WordleWord> GuessableWords => _wordleDictionary.AllowedWords;

    public DrawingSolver(IWordleDictionary wordleDictionary)
    {
        _wordleDictionary = wordleDictionary;
    }

    public DrawingSolutionDTO Solve(BoardClue userDrawing)
    {
        List<CategorySolutionResult> categorySolutions = [];
        DrawingValidation validate = ValidateDrawing(userDrawing);
        if  (validate != DrawingValidation.Valid)
        {
            return new(validate, categorySolutions);
        }

        CategorySolutionResult exactMatchSolution = ExactSolve(userDrawing);
        /*
        CategorySolutionResult shapeMatchSolution = ShapeSolve(userDrawing);
        CategorySolutionResult missOneMatchSolution = MissOneSolve(userDrawing);
        */

        categorySolutions.Add(exactMatchSolution);
        /*
        categorySolutions.Add(shapeMatchSolution);
        categorySolutions.Add(missOneMatchSolution);
        */

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
    private CategorySolutionResult MissOneSolve(BoardClue userDrawing)
    {
        throw new NotImplementedException();
    }

    // TODO_LOW: This will need to create a list of rows it wants to make, make them, and then construct solutions from the combinations of row solutions.
    private CategorySolutionResult ShapeSolve(BoardClue userDrawing)
    {
        throw new NotImplementedException();
    }

    // TODO_LOW: Make this re-use solutions if multiple rows have same value.
    private CategorySolutionResult ExactSolve(BoardClue userDrawing)
    {
        string[] regexConditions = new string[6];
        
        // Create regex conditions for check on guessable words
        for (int rowIndex = 0; rowIndex < 6; rowIndex++)
        {  
            WordClue rowDrawing = userDrawing[rowIndex];
            HashSet<char> greenedOut = GreenedOut(rowDrawing);

            string regexInternal = "";
            for (int letterIndex = 0; letterIndex < 5; letterIndex++)
            {
                regexInternal += RegexMaker(letterIndex, greenedOut, rowDrawing);
            }

            regexConditions[rowIndex] = "^" + regexInternal + "$";
        }

        // Check guessable words for words that match conditions
        SolutionWords solutionWords = new();
        for (int rowIndex = 0; rowIndex < 6; rowIndex++)
        {
            string condition = regexConditions[rowIndex];
            solutionWords[rowIndex] = GuessableWords
                                                .Where(s => Regex.IsMatch(s.ToString(), condition))
                                                .ToHashSet();
        }


        SolutionType solutionType = SolutionType.Exact;
        List<Solution> solutions = [];

        Solution solution = new(userDrawing, solutionWords);
        solutions.Add(solution);
        return new(solutionType, solutions);
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

            // 2. Count how many times each letter is greened.
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
}
