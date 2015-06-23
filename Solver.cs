using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace _15puzzle
{
    internal delegate void PuzzleSolution(int steps, int time, Stack<Direction> path);

    class Solver
    {
        private int[] _gameField;
        private int _steps;
        private readonly Stack<Direction> _resPath;
        private readonly Heuristic _heuristic;
        private readonly Stopwatch _stopWatch;
        
        internal event PuzzleSolution PuzzleSolved;
        
        internal Solver(Heuristic heuristic)
        {
            _heuristic = heuristic;
            _steps = 0;
            _resPath = new Stack<Direction>();
            _stopWatch = new Stopwatch();
        }

        internal void Solve(int[] nodes)
        {
            ThreadPool.QueueUserWorkItem(item => StartIda(nodes));
        }


        public void StartIda(int[] nodes)
        {
           _gameField = nodes;
            StartMeasure();
            int bound = _heuristic.H(_gameField);
            int spaceIndex = -1;
            bool result = false;
            while (!result)
            {
                for (int i = 0; i < _gameField.Length; i++)
                    if (_gameField[i] == PuzzleGame.SpaceValue)
                        spaceIndex = i;
                _steps = 0;
                result = IterativeDeepeningASearch(0,Direction.None, spaceIndex,bound);

                bound += 2;
            }
            EndMeasure();
            if (PuzzleSolved != null) PuzzleSolved(_steps, (int)_stopWatch.ElapsedMilliseconds, _resPath);
        }

        private bool IterativeDeepeningASearch(int g, Direction prevDir, int spaceIndex, int currentCostBound)
        {
            var h = _heuristic.H(_gameField);
            if (h == 0)
                return true;

            var f = g + h;
            if (f > currentCostBound)
                return false;

            foreach (Direction direction in Enum.GetValues(typeof (Direction)))
            {
                if((prevDir + (int) direction) == 0 || direction == Direction.None)
                    continue;
                int newPosition;
                if (MakeMove(direction, spaceIndex, out newPosition))
                {
                    Swap(_gameField, spaceIndex, newPosition);
                    var res = IterativeDeepeningASearch(g + 1, direction, newPosition, currentCostBound);
                    Swap(_gameField, newPosition, spaceIndex);
                    if (!res) continue;
                    _resPath.Push(direction);
                    _steps++;
                    return true;
                }
                
            }
            return false;
        }
        private void Swap(int[] nodes, int i, int j)
        {
            int t = nodes[i];
            nodes[i] = nodes[j];
            nodes[j] = t;
        }

        private bool MakeMove(Direction direction, int spaceIndex, out int newPosition)
        {
            int gridSize = (int)Math.Sqrt(_gameField.Length);
            int currentX = spaceIndex % gridSize;
            int currentY = spaceIndex / gridSize;
            newPosition = -1;

            switch (direction)
            {
                case Direction.Up:
                    {
                        if (currentY != 0)
                        {
                            newPosition = spaceIndex - gridSize;
                        }
                    }
                    break;

                case Direction.Down:
                    {
                        if (currentY < (gridSize - 1))
                        {
                            newPosition = spaceIndex + gridSize;
                        }
                    }
                    break;

                case Direction.Left:
                    {
                        if (currentX != 0)
                        {
                            newPosition = spaceIndex - 1;
                        }
                    }
                    break;

                case Direction.Right:
                    {
                        if (currentX < (gridSize - 1))
                        {
                            newPosition = spaceIndex + 1;
                        }
                    }
                    break;
            }

            return newPosition != -1;
        }
       private void StartMeasure()
        {
            _stopWatch.Reset();
            _stopWatch.Start();
        }
        private void EndMeasure()
        {
            _stopWatch.Stop();
        }
    }
}
