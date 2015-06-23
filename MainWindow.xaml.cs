using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace _15puzzle
{
    internal delegate void PuzzleSolved();
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {

        private readonly PuzzleGame _game;
        private Solver _solver;
        private bool _working;
        private int _stepsCount;
        public int StepsCount
        {
            get { return _stepsCount; }
            set
            {
                _stepsCount = value;
                OnPropertyChanged("StepsCount");
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            StatusTextBlock.Text = "Press <Solve> to show the solution...";
            _working = false;
            _stepsCount = 0;
            _game = new PuzzleGame(CanvBoard, 4);
            _game.PuzzleClick += PzlCellMouseLeftButtonUp;
            _game.PuzzleSolved += GameOnPuzzleSolved;
            _game.InitNewGame();

        }

        private void GameOnPuzzleSolved()
        {
            MessageBox.Show("Congratulations! U did it in " + StepsCount + " steps!\nPress <New Game> to fight again", "VICTORY", MessageBoxButton.OK,
                MessageBoxImage.Information);
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
            {
                StepsCount++;
                _game.MoveCell(cell);
            }
        }

        private void NewGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_working)
                return;
            _working = true;
            StepsCount = 0;
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
            
            _solver = new Solver(new Heuristic());
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
                if (MessageBox.Show("Show solution steps?", "Solution found!", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes)
                {
                    while (path.Count > 0)
                    {
                        _game.MoveSpaceCell(path.Pop());
                        StepsCount++;
                    }
                }    
            }
            
            _working = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
