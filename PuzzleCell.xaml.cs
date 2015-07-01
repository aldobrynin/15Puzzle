namespace _15puzzle
{
    /// <summary>
    /// Логика взаимодействия для PuzzleCell.xaml
    /// </summary>
    public partial class PuzzleCell
    {
        public int Number { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public PuzzleCell(int num)
        {
            
            InitializeComponent();

            TbValue.Text = num == PuzzleGame.SpaceValue ? string.Empty : num.ToString();
        }
    }
}
