using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour.Model
{
    public class Board
    {
        public char[,] Tokens;

        public int Height { get; }

        public int Width { get; }

        // number of empty rows at the top of the board
        // convenience property for later
        public int EmptyRows { get; set; }



        public Board(int height=6, int width=7)
        {
            Height = height; Width = width;
            Tokens = new char[height, width];

            EmptyRows = height;

            // Initialize the board with default '-'
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    Tokens[row, col] = '-';
                }
            }
        }

        /// <summary>
        /// Place the designated player's token in the designated column
        /// </summary>
        /// <param name="col"></param> 
        ///     0 < col < Board.Width
        /// <param name="player"></param>
        ///     player is 'R' or 'Y'
        public void PlaceToken(int col, char player)
        {
            for (int row = Tokens.GetLength(0) - 1; row >= 0; row--)
            {
                if (Tokens[row, col] == '-')
                {
                    // check if token was placed in empty row
                    EmptyRows = Math.Min(EmptyRows, row);

                    Tokens[row, col] = player; 
                    break;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            for (int row  = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    sb.Append(Tokens[row, col]);
                    if (col < Width - 1)
                        sb.Append(',');
                }

                sb.Append('\n');
            }
            
            return sb.ToString();
        }
    }
}
