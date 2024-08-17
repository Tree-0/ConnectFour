using ConnectFour.Model;
using Microsoft.Windows.Themes;
using System.ComponentModel;
using System.Data.Common;
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
    /*
     * BUGS: 
     * 
    */

    /*
     * TODO:
     * 
     * ADD ARROW KEYS AND ENTER KEY FUNCTIONALITY
     * 
     */


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
        /// boolean that marks the game as finished when a winner is found.
        /// Used to prevent further moves until restart
        /// </summary>
        private bool _isGameOver { get; set; }


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
            _isGameOver = false;

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
        /// Add a token to the BoardGrid in the designated row and column. Returns true if successful.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private bool AddTokenToGridCell(int row, int column)
        {
            // Create an Ellipse (circle)
            Ellipse ellipse = new Ellipse
            {
                Width = BoardGrid.Width / 7, // Set width
                Height = BoardGrid.Width / 7, // Set height
                Fill = (PlayerTurn == 'Y') ? Brushes.LightGoldenrodYellow : Brushes.Coral,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Focusable = false
            };

            // Position the Ellipse in the specified Grid cell if row is not full
            if (row >= 0)
            {
                Grid.SetRow(ellipse, row);
                GameNameTextBox.Text = "Connect 4";
            }
            // row is full
            else
            {
                return false;
            }
                        
            Grid.SetColumn(ellipse, column);

            // Add the Ellipse to the Grid's children
            BoardGrid.Children.Add(ellipse);
            return true;
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

            // Nothing should happen if a winner has already been determined
            if (_isGameOver)
            {
                return;
            }

            // Nothing should happen if no cell selected or if column is full
            if (_selectedBorder == null)
            {
                GameNameTextBox.Text = "Select a valid cell";                
                return;
            }

            // Place Token 
            int col = Grid.GetColumn(_selectedBorder);
            int row = Board.PlaceToken(col, PlayerTurn);

            // if column is full token will not add to model
            if (row == -1)
            {
                GameNameTextBox.Text = "Column is full";
                return;
            }

            // Remove preview token to prevent visual inconsistency
            RemoveTokenPreviewFromGridCell(row, col);

            // Add token to the grid visually
            if (!AddTokenToGridCell(row, col))
            {
                GameNameTextBox.Text = "Failed to add token to the grid";
                return;
            }

            // Move finished, increase turn count
            TurnNumber++;

            // Check for a winner
            char winner = Board.CheckForWin();
            if (winner == 'Y')
            {
                GameNameTextBox.Text = "Yellow Wins";
                _isGameOver = true;
            }
            else if (winner == 'R')
            {
                GameNameTextBox.Text = "Red Wins";
                _isGameOver = true;
            }
            else
            {
                GameNameTextBox.Text = "Connect 4";
            }

            // Prevent accidentally stacking multiple tokens on each other
            _selectedBorder = null;

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

            // Revert Win Message
            GameNameTextBox.Text = "Connect 4";

            // New game, Turn = 0
            TurnNumber = 0;
            PlayerTurn = 'R';
            _isGameOver = false;
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
        /// When a cell is clicked, change the border color and place a preview token
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
                    selectedBorder.BorderBrush = (PlayerTurn == 'R') ? Brushes.Red : Brushes.Yellow;
                    selectedBorder.Background = Brushes.LightGray;
                    Panel.SetZIndex(selectedBorder, -1);
                }

                int col = Grid.GetColumn(selectedBorder);
                int row = Board.GetLowestEmptyRow(col);
                if (row != -1)
                {
                    AddTokenPreviewToGridCell(row, col);
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

                // remove preview token from de-selected cell
                int col = Grid.GetColumn(selectedBorder);
                int row = Board.GetLowestEmptyRow(col);
                RemoveTokenPreviewFromGridCell(row, col);

            }
        }


        /// <summary>
        /// Adds a token preview to the selected cell as a visual aid. Not final, becomes final when confirm button is clicked
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private void AddTokenPreviewToGridCell(int row, int column)
        {
            // Create an Ellipse (circle)
            Ellipse ellipse = new Ellipse
            {
                Width = BoardGrid.Width / 7, // Set width
                Height = BoardGrid.Width / 7, // Set height
                Fill = (PlayerTurn == 'Y') ? Brushes.LightGoldenrodYellow : Brushes.Coral,
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                Focusable = false,
                Tag = "preview"
            };

            // Check if an ellipse preview already exists at the row and column
            foreach (UIElement element in BoardGrid.Children)
            {
                if (Grid.GetRow(element) == row && Grid.GetColumn(element) == column
                    && element is Ellipse existing && existing.Tag != null
                    && existing.Tag.ToString() == "preview")
                {
                    // Do nothing, return
                    return;
                }
            }

            Grid.SetRow(ellipse, row);
            Grid.SetColumn(ellipse, column);
            Grid.SetZIndex(ellipse, 1);
            BoardGrid.Children.Add(ellipse);
        }

        /// <summary>
        /// Remove the preview token from the specified cell.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private void RemoveTokenPreviewFromGridCell(int row, int column)
        {
            UIElement previewTokenToRemove = null;

            // Iterate over the Grid's children to find the preview token in the specified cell
            foreach (UIElement element in BoardGrid.Children)
            {
                if (Grid.GetRow(element) == row && Grid.GetColumn(element) == column 
                    && element is Ellipse ellipse && ellipse.Tag != null 
                    && ellipse.Tag.ToString() == "preview")
                {
                    previewTokenToRemove = element;
                    break;
                }
            }

            // If the preview token was found, remove it from the Grid
            if (previewTokenToRemove != null)
            {
                BoardGrid.Children.Remove(previewTokenToRemove);
            }
        }

        // Enter key pressed -> trigger confirmMove button
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConfirmMoveButton_Click(ConfirmMoveButton, null);
                e.Handled = true; // Mark the event as handled to prevent further processing
            }
        }

    }
}