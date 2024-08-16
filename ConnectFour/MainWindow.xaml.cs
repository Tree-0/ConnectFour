using ConnectFour.Model;
using Microsoft.Windows.Themes;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConnectFour
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // model of board, containing all tokens
        public Board Board { get; set; }

        /// <summary>
        /// TurnNumber: the number of turns played in the current game
        /// </summary>
        private int _turnNumber;
        public int TurnNumber
        {
            get { return _turnNumber; }
            set
            {
                if (value != _turnNumber)
                {
                    _turnNumber = value;
                    OnPropertyChanged(nameof(TurnNumber));
                }
            }
        }

        /// <summary>
        /// PlayerTurn: the player who is placing a token next
        /// </summary>
        private char _playerTurn;
        public char PlayerTurn
        {
            get { return _playerTurn; }
            set
            {
                if (value != _playerTurn)
                {
                    _playerTurn = value;
                    OnPropertyChanged(nameof(PlayerTurn));
                }
            }
        }

        /// <summary>
        /// When a cell is clicked, that is the selected Cell with a respective
        /// selected border.
        /// When a cell loses focus, it is no longer the selected border
        /// </summary>
        private Border? _selectedBorder { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            // Init model
            Board = new Board();
            TurnNumber = 0;
            PlayerTurn = 'R';

        }

        /// <summary>
        /// PropertyChanged: Event for DataBinding
        /// OnPropertyChanged: Raises event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Add a token to the BoardGrid in the designated row and column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private void AddTokenToGridCell(int row, int column)
        {
            // Create an Ellipse (circle)
            Ellipse ellipse = new Ellipse
            {
                Width = BoardGrid.Width / 7, // Set width
                Height = BoardGrid.Width / 7, // Set height
                Fill = (PlayerTurn == 'Y') ? Brushes.LightGoldenrodYellow : Brushes.Coral,
                Stroke = Brushes.Black,
                StrokeThickness = 2 
            };

            // Position the Ellipse in the specified Grid cell
            Grid.SetRow(ellipse, row);
            Grid.SetColumn(ellipse, column);

            // Add the Ellipse to the Grid's children
            BoardGrid.Children.Add(ellipse);
        }


        /// <summary>
        /// Remove a token from the BoardGrid in the designated row and column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private void RemoveTokenFromGridCell(int row, int column)
        {
            UIElement tokenRemove = null;

            foreach (UIElement token in BoardGrid.Children)
            {
                if (Grid.GetColumn(token) == column && Grid.GetRow(token) == row)
                {
                    tokenRemove = token;
                    break;
                }
            }

            if (tokenRemove != null)
            {
                BoardGrid.Children.Remove(tokenRemove);
            }
        }


        /// <summary>
        /// Remove all tokens from the BoardGrid
        /// </summary>
        private void RemoveAllTokensFromGrid()
        {
            for (int i = BoardGrid.Children.Count - 1; i >= 0; i--)
            {
                UIElement element = BoardGrid.Children[i];
                if (element is Ellipse)
                    BoardGrid.Children.Remove(element);
            }
        }


        /// <summary>
        /// Make the previewed move final- place token, increment turn counter, switch teams
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmMoveButton_Click(object sender, RoutedEventArgs e)
        {
            // Move finished, increase turn count
            TurnNumber++;

            // Place Token 
            int col = Grid.GetColumn(_selectedBorder);
            int row = Board.PlaceToken(col, PlayerTurn);
            AddTokenToGridCell(row, col);

            // Switch player
            PlayerTurn = (PlayerTurn == 'Y') ? 'R' : 'Y';
        }


        /// <summary>
        /// Remove all tokens from the board and set turn counter to zero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            // Remove visual tokens
            RemoveAllTokensFromGrid();

            // Remove model tokens
            Board.RemoveAllTokens();

            // New game, Turn = 0
            TurnNumber = 0;
            PlayerTurn = 'R';
        }


        /// <summary>
        /// Deselect Cell and remove token preview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelMoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBorder != null)
            {
                _selectedBorder.BorderBrush = Brushes.Black;
                _selectedBorder.Background = Brushes.Transparent;
            }

            _selectedBorder = null;
        }


        /// <summary>
        /// When a cell is clicked, change the border color and place a token
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridCell_GotFocus(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                Border selectedBorder = clickedButton.Parent as Border;
                _selectedBorder = selectedBorder;

                // Highlight the selected border
                if (selectedBorder != null)
                {
                    selectedBorder.BorderBrush = Brushes.Red;
                    selectedBorder.Background = Brushes.LightGray;
                }
            }
        }


        /// <summary>
        /// When a different cell is clicked, revert the de-selected cell to its original state
        /// </summary>
        /// <param name="sender"></param> 
        /// <param name="e"></param>
        private void GridCell_LostFocus(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                Border selectedBorder = clickedButton.Parent as Border;

                // Un-highlight the de-selected border
                if (selectedBorder != null)
                {
                    selectedBorder.BorderBrush= Brushes.Black;
                    selectedBorder.Background = Brushes.Transparent;
                }

            }
        }
    }
}