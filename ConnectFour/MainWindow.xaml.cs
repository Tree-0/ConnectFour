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
                if (_turnNumber != value)
                {
                    _turnNumber = value;
                    OnPropertyChanged(nameof(TurnNumber));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            // Init model
            Board = new Board();

            // //Test
            // Board.PlaceToken(0, 'Y');
            // Board.PlaceToken(1, 'R');
            // Board.PlaceToken(1, 'Y');
            // Debug.WriteLine(Board.ToString());
            // Debug.WriteLine(Board.EmptyRows);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        }


        /// <summary>
        /// Remove all tokens from the board and set turn counter to zero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            // New game, Turn = 0
            TurnNumber = 0;
        }


        /// <summary>
        /// Deselect Cell and remove token preview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelMoveButton_Click(object sender, RoutedEventArgs e)
        {

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