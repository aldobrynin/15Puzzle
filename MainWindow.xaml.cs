using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace _15puzzle
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private readonly PuzzleGame _game;
        private Solver _solver;
        private bool _working;
        public MainWindow()
        {
            InitializeComponent();
            StatusTextBlock.Text = "Press <Shuffle> to start the game...";
            _working = false;
            _game = new PuzzleGame(CanvBoard, 4);
            _game.PuzzleClick += PzlCellMouseLeftButtonUp;
            _game.InitNewGame();

        }


        private void PzlCellMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (_working)
                return;
            var cell = sender as PuzzleCell;
            if (cell == null)
                return;

            int spaceCol = _game.SpaceIndex%_game.Size;
            int spaceRow = _game.SpaceIndex/_game.Size;
            
            if (Math.Abs(spaceCol - cell.Column) + Math.Abs(spaceRow - cell.Row) == 1)
                _game.MoveCell(cell);
        }

        private void NewGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_working)
                return;
            _working = true;
            StatusTextBlock.Text = "Shuffling...";
            _game.InitNewGame();
            StatusTextBlock.Text = "Click cells to move 'em...";
            _working = false;
        }

        private void SolveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_working)
                return;
            _working = true;
            StatusTextBlock.Text = "Solving. Please wait...";
            _solver = new Solver();
            _solver.PuzzleSolved += SolverOnPuzzleSolved;
            _solver.Solve(_game.GameField);
        }

        private void SolverOnPuzzleSolved(int steps, int time, Stack<Direction> path)
        {
            string text;
            if (steps == -1)
            {
                text = "No solution found :(. Time elapsed: " + time/1000.0 + " seconds";
            }
            else
            {
                text = "Solution found in " + time/1000.0 + " seconds (total " + steps + " steps)!";
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                StatusTextBlock.Text = text;
            });
            if (steps == -1)
            {
                MessageBox.Show("No solution found");
            }
            else
            {
                if (MessageBox.Show("Show solution steps?", "Solution found!", MessageBoxButton.YesNo) ==
                MessageBoxResult.Yes)
                {
                    while (path.Count > 0)
                    {
                        _game.MoveSpaceCell(path.Pop());
                    }
                }    
            }
            
            _working = false;
        }
    }
}
