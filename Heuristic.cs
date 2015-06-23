using System;

namespace _15puzzle
{
    /// <summary>
    /// Вычисление значения эвристической функции
    /// </summary>
    class Heuristic
    {
        public int H(int[] gameField)
        {
            return GetManhattanDistanceCost(gameField) + GetLinearConflictCost(gameField);
        }

        /// <summary>
        /// Манхэттэнское расстояние
        /// </summary>
        /// <param name="gameField"></param>
        /// <returns></returns>
        private int GetManhattanDistanceCost(int[] gameField)
        {
            var heuristicCost = 0;
            var size = (int)Math.Sqrt(gameField.Length);

            for (int i = 0; i < gameField.Length; i++)
            {
                var value = gameField[i] - 1;
                if (value == PuzzleGame.SpaceValue - 1)
                {
                    continue;
                }

                if (value != i)
                {
                    var idealX = value % size;
                    var idealY = value / size;

                    var currentX = i % size;
                    var currentY = i / size;

                    heuristicCost += (Math.Abs(idealY - currentY) + Math.Abs(idealX - currentX));
                }
            }
            return heuristicCost;
        }

        /// <summary>
        /// Стоимость линейных конфликтов
        /// </summary>
        /// <param name="gameField"></param>
        /// <returns></returns>
        private int GetLinearConflictCost(int[] gameField)
        {
            var heuristicCost = 0;
            var size = (int)Math.Sqrt(gameField.Length);
            //по строкам
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    var thisCell = gameField[row * size + col];
                    if (thisCell == PuzzleGame.SpaceValue)
                        continue;

                    var thisCellIdealRow = (thisCell - 1) / size;
                    if (row != thisCellIdealRow)
                        continue;
                    for (int k = row * size + col + 1; k < row * size + size; k++)
                    {
                        var thatCell = gameField[k];
                        if (thatCell == PuzzleGame.SpaceValue)
                            continue;

                        var thatCellIdealRow = (thatCell - 1) / size;
                        if (thatCellIdealRow == thisCellIdealRow && thisCell > thatCell)
                            heuristicCost += 2;
                    }
                }
            }
            //по столбцам
            for (int col = 0; col < size; col++)
            {
                for (int row = 0; row < size; row++)
                {
                    var thisCell = gameField[row * size + col];
                    if (thisCell == PuzzleGame.SpaceValue)
                        continue;

                    var thisCellIdealCol = (thisCell - 1) % size;
                    if (row != thisCellIdealCol)
                        continue;
                    for (int k = (row + 1) * size + col; k < size * size + col; k += size)
                    {
                        var thatCell = gameField[k];
                        if (thatCell == PuzzleGame.SpaceValue)
                            continue;
                        var thatCellIdealCol = (thatCell - 1) % size;
                        if (thatCellIdealCol == thisCellIdealCol && thisCell > thatCell)
                            heuristicCost += 2;
                    }
                }
            }
            return heuristicCost;
        }
    }
}
