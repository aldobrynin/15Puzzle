using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace _15puzzle
{
    /// <summary>
    /// Общий класс игры
    /// </summary>
    class PuzzleGame
    {

        public const int SpaceValue = 0;
        
        public int[] GameField;
        public readonly int Size;
        public int SpaceIndex;

        private readonly Panel _panel;
        public event MouseButtonEventHandler PuzzleClick;
        
        public PuzzleGame(Panel panel, int size)
        {
            _panel = panel;
            Size = size;
            GameField = new int[size * size];

        }
        public void InitNewGame()
        {
            for (int i = 0; i < Size*Size; i++)
                GameField[i] = i == Size*Size - 1 ? SpaceValue : i + 1;
            
            SpaceIndex = Size * Size - 1;
            Shuffle();
            DrawField();
        }

        private void Shuffle()
        {
            int seed = 37 + 37 * ((int)DateTime.Now.TimeOfDay.TotalSeconds % 37);
            Random random = new Random(seed);

            do
            {
                for (int i = 0; i < GameField.Length; i++)
                {
                    var position = random.Next(0, i);
                    Swap(GameField, i, position);
                    if (GameField[position] == SpaceValue)
                        SpaceIndex = position;
                }
            } while (!IsSolvable());
        }

        private bool IsSolvable()
        {

            int numOfTiles = GameField.Length,
            dim = (int)Math.Sqrt(numOfTiles);
            int inversions = 0;

            for (int i = 0; i < numOfTiles; ++i) 
            {
                int iTile = GameField[i];
                if (iTile != SpaceValue) 
                {
                    for (int j = i + 1; j < numOfTiles; ++j) 
                    {
                        int jTile = GameField[j];
                        if (jTile != SpaceValue && jTile < iTile)
                        {
                            ++inversions;
                        }
                    }
                } 
                else if ((dim & 0x1) == 0) {
                    inversions += (1 + i / dim);
                }
            }
            return (inversions & 0x1) != 1;
        }

        private int GetNewSpacePosition(Direction direction)
        {
            int spaceRow = SpaceIndex / Size;
            int spaceCol = SpaceIndex % Size;
            int cellRow = -1;
            int cellCol = -1;
            switch (direction)
            {
                case Direction.Down:
                    if (spaceRow < Size - 1)
                    {
                        cellCol = spaceCol;
                        cellRow = spaceRow + 1;
                    }
                    break;
                case Direction.Up:
                    if (spaceRow > 0)
                    {
                        cellCol = spaceCol;
                        cellRow = spaceRow - 1;
                    }
                    break;
                case Direction.Left:
                    if (spaceCol > 0)
                    {
                        cellCol = spaceCol - 1;
                        cellRow = spaceRow;
                    }
                    break;
                case Direction.Right:
                    if (spaceCol < Size - 1)
                    {
                        cellCol = spaceCol + 1;
                        cellRow = spaceRow;
                    }
                    break;
            }
            if (cellCol == -1 || cellRow == -1)
                return -1;
            return cellRow*Size + cellCol;
        }
        private void Swap(int[] a, int i, int j)
        {
            int temp = a[i];
            a[i] = a[j];
            a[j] = temp;
        }
        private void DrawField()
        {
            _panel.Children.Clear();
            for (var i = 0; i < Size * Size; i++)
            {
                if (GameField[i] == SpaceValue) continue;

                var pzlCell = new PuzzleCell(GameField[i])
                {
                    Number = GameField[i],
                    Width = _panel.Width / Size,
                    Height = _panel.Height / Size,
                    Row = i / Size,
                    Column = i % Size
                };
                Canvas.SetTop(pzlCell, pzlCell.Row * pzlCell.Height);
                Canvas.SetLeft(pzlCell, pzlCell.Column * pzlCell.Width);
                _panel.Children.Add(pzlCell);
                pzlCell.MouseLeftButtonUp += PuzzleClick;
            }
        }

        public void MoveCell(PuzzleCell pCell)
        {
            var buttonAnimation = new DoubleAnimation();
            int spaceRow = SpaceIndex/Size;
            int spaceCol = SpaceIndex%Size;
            if (pCell.Row == spaceRow)
            {
                buttonAnimation.From = pCell.Column * pCell.Width;
                buttonAnimation.To = spaceCol * pCell.Width;
                buttonAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));

                pCell.BeginAnimation(Canvas.LeftProperty, buttonAnimation);
            }
            else
            {
                buttonAnimation.From = pCell.Row * pCell.Height;
                buttonAnimation.To = spaceRow * pCell.Height;
                buttonAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));

                pCell.BeginAnimation(Canvas.TopProperty, buttonAnimation);
            }
            var newIndex = pCell.Row*Size + pCell.Column;
            GameField[newIndex] = GameField[SpaceIndex];
            GameField[SpaceIndex] = pCell.Number;

            SpaceIndex = newIndex;
            pCell.Row = spaceRow;
            pCell.Column = spaceCol;
        }

        public bool MoveSpaceCell(Direction direction)
        {
            var newIndex = GetNewSpacePosition(direction);
            if (newIndex == -1)
                return false;
            
            Application.Current.Dispatcher.Invoke(() =>
            {
                var cell = _panel.Children.OfType<PuzzleCell>().First(x => x.Row == newIndex/Size && x.Column == newIndex % Size);
                MoveCell(cell);
            });

            Thread.Sleep(300);
            return true;

        }
    }
}
