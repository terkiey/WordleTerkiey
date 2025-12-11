using System.Runtime.CompilerServices;

namespace API;

internal class BoardMapper : IBoardMapper
{
    Dictionary<BoardClue, List<BoardClue>> _cachedShapeMaps = [];
    Dictionary<BoardClue, List<BoardClue>> _cachedMissOneMaps = [];
    Dictionary<BoardClue, List<BoardClue>> _cachedMirrorMaps = [];

    public List<BoardClue> MapToShape(BoardClue boardClue)
    {
        if (_cachedShapeMaps.TryGetValue(boardClue, out List<BoardClue>? cachedMap))
        {
            return cachedMap;
        }

        Palette colors = new([]);
        colors.ColorsInOrder.Add(BoxColor.Black);
        colors.ColorsInOrder.Add(BoxColor.Yellow);
        colors.ColorsInOrder.Add(BoxColor.Green);

        List<Palette> paletteList = [];
        for (int firstColorIndex = 0; firstColorIndex < 3; firstColorIndex++)
        {
            BoxColor firstColor = colors.ColorsInOrder[firstColorIndex];
            for (int secondColorGap = 1; secondColorGap <= 2; secondColorGap++)
            {
                int secondColorIndex = (firstColorIndex + secondColorGap) % 3;
                BoxColor secondColor = colors.ColorsInOrder[secondColorIndex];

                int thirdColorGap = 3 - secondColorGap;
                int thirdColorIndex = (firstColorIndex + thirdColorGap) % 3;
                BoxColor thirdColor = colors.ColorsInOrder[thirdColorIndex];

                List<BoxColor> iterationColors = [];
                iterationColors.Add(firstColor);
                iterationColors.Add(secondColor);
                iterationColors.Add(thirdColor);
                Palette iterationPalette = new(iterationColors);
                paletteList.Add(iterationPalette);
            }
        }

        List<BoardClue> outputBoards = [];
        foreach(Palette palette in paletteList)
        {
            if (palette == colors) { continue; }
            BoardClue blankBoard = new();
            var paletteColors = palette.ColorsInOrder;

            for (int rowIndex = 0; rowIndex < 6; rowIndex++)
            {
                WordClue blankWord = blankBoard[rowIndex];
                WordClue boardRow = boardClue[rowIndex];
                for (int colIndex = 0; colIndex < 5; colIndex++)
                {
                    switch (boardRow[colIndex])
                    {
                        case BoxColor.Black:
                            blankWord[colIndex] = paletteColors[0];
                            break;

                        case BoxColor.Yellow:
                            blankWord[colIndex] = paletteColors[1];
                            break;

                        case BoxColor.Green:
                            blankWord[colIndex] = paletteColors[2];
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(boardClue), $"Board has an invalid boxcolor.");
                    }
                }
            }

            if (blankBoard != boardClue) { outputBoards.Add(blankBoard); }
        }

        _cachedShapeMaps.Add(boardClue, outputBoards);
        return outputBoards;
    }

    public List<BoardClue> MapToMissOne(BoardClue boardClue)
    {
        if (_cachedMissOneMaps.TryGetValue(boardClue, out List<BoardClue>? cachedMap))
        {
            return cachedMap;
        }

        List<BoardClue> outputBoards = [];
        for (int rowIndex = 0; rowIndex < 6; rowIndex++)
        {
            for (int colIndex = 0; colIndex < 5; colIndex++)
            {
                BoardClue missOneBoard = new BoardClue(boardClue);

                if (missOneBoard[rowIndex][colIndex] == BoxColor.Black) { continue; }
                missOneBoard[rowIndex][colIndex] = BoxColor.Black;
                outputBoards.Add(missOneBoard);
            }
        }

        outputBoards.Select(bc => bc != boardClue);
        return outputBoards;
    }

    public List<BoardClue> MapToMirrors(BoardClue boardClue)
    {
        if (_cachedMirrorMaps.TryGetValue(boardClue, out List<BoardClue>? cachedMap))
        {
            return cachedMap;
        }

        List<BoardClue> outputBoards = [];
        outputBoards.Add(HorizontalMirror(VerticalMirror(boardClue)));
        outputBoards.Add(HorizontalMirror(boardClue));
        outputBoards.Add(VerticalMirror(boardClue));

        return outputBoards;
    }

    private BoardClue VerticalMirror(BoardClue boardClue)
    {
        WordClue[] newRows = new WordClue[6];
        for (int rowIndex = 0; rowIndex < 6;  rowIndex++)
        {
            newRows[rowIndex] = MirrorRow(boardClue[rowIndex]);
        }

        return new(newRows);
    }

    private BoardClue HorizontalMirror(BoardClue boardClue)
    {
        WordClue[] newRows = new WordClue[6];
        for (int rowIndex = 0; rowIndex < 6 ; rowIndex++)
        {
            newRows[rowIndex] = boardClue[5 - rowIndex];
        }

        return new BoardClue(newRows);
    }

    private WordClue MirrorRow(WordClue row)
    {
        WordClue outputRow = new();
        for (int letterIndex = 0; letterIndex < 5; letterIndex++)
        {
            outputRow[(row.LetterClues.Length - letterIndex) - 1] = row[letterIndex];
        }

        return outputRow;
    }
}
