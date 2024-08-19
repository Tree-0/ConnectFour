using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour.Model
{
    /*TODO
     * 
     * - MAKE GAME MORE VISUALLY SMOOTH AND APPEALING
     *      - Weird Grid Borders
     *      - Animation for dropping token?
     */



    public class Board
    {
        public char[,] Tokens { get; set; }

        public int Height { get; }

        public int Width { get; }

        // number of empty rows at the top of the board
        // a surprise tool that will help us later
        public int EmptyRows { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="height"></param> 
        /// <param name="width"></param>
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

        public Board(char[,] tokens)
        {
            Tokens = tokens;
            Height = tokens.GetLength(0);
            Width = tokens.GetLength(1);
            
            
        }


        /// <summary>
        /// Place the designated player's token in the designated column, 
        /// return the row in that column where the token was placed
        /// </summary>
        /// <param name="col"></param> 
        ///     0 < col < Board.Width
        /// <param name="player"></param>
        ///     player is 'R' or 'Y'
        public int PlaceToken(int col, char player)
        {
            int rowPlaced = -1;

            for (int row = Tokens.GetLength(0) - 1; row >= 0; row--)
            {
                if (Tokens[row, col] == '-')
                {
                    // check if token was placed in empty row
                    EmptyRows = Math.Min(EmptyRows, row);

                    // store the row this token was placed in
                    rowPlaced = row;

                    // add to model
                    Tokens[row, col] = player; 
                    break;
                }
            }

            return rowPlaced;
        }


        /// <summary>
        /// In a particular column, get the row index of the lowest empty space in that column
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public int GetLowestEmptySpace(int col)
        {
            for (int row = Height - 1; row >= 0; row--)
            {
                if (Tokens[row, col] == '-')
                {
                    return row;
                }
            }

            // No Empty Rows
            return -1; 
        }


        /// <summary>
        /// Get the number of empty rows at the top of a board... will be the minimum 
        /// value of GetLowestEmptySpace for every column
        /// </summary>
        /// <returns></returns>
        public int GetEmptyRows()
        {
            int emptyRows = Height;

            for (int col = 0; col < Width; col++)
            {
                int emptyRowsInCol = GetLowestEmptySpace(col);

                // if a column is full, the board has no empty rows
                if (emptyRowsInCol == -1) return 0;

                // if current column has fewer empty rows than the current count, count must shrink
                if (emptyRowsInCol < emptyRows)
                    emptyRows = emptyRowsInCol;
            }

            return emptyRows;
        }


        /// <summary>
        /// Remove the token at the specified index. If successfully removed, return True.
        /// If there was nothing to remove, return False. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool RemoveToken(int row, int col)
        {
            if (Tokens[row, col] == '-')
                return false;

            Tokens[row, col] = '-';
            return true;
        }


        /// <summary>
        /// Remove every token on the board. Always returns true.
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllTokens()
        {
            for(int row = 0; row < Height; row++)
            {
                for(int col = 0; col < Width; col++)
                {
                    RemoveToken(row, col);
                }
            }

            EmptyRows = Height;

            return true;
        }


        /// <summary>
        /// Check the board to see if any player has gotten 4 tokens in a row.
        /// Return 'R', 'Y', or '-' if there was no winner.
        /// </summary>
        /// <returns></returns>
        public char CheckForWin()
        {
            /*
             * x x x x x x x
             * x x x x x x x
             * x x x x x x x
             * x x x x x x x 
             * x x x x x x x
             * x x x x x x x
            */

            Debug.WriteLine(ToString());

            // Check 4 across
            for (int row = Height - 1; row >= 0; row--)
            {
                for(int col = 0; col < Width - 3; col++)
                {
                    if (IsFourAcross(row, col)) return Tokens[row,col];
                }
            }

            // Check 4 up
            for (int row = Height - 1; row >= 3; row--)
            {
                for (int col = 0; col < Width; col++)
                {
                    if (IsFourUp(row, col)) return Tokens[row, col];
                }
            }

            // Check 4 diagonal up
            for (int row = Height - 1; row >= 3; row--)
            {
                for (int col = 0; col < Width - 3; col++)
                {
                    if (IsFourDiagonalUp(row,col)) return Tokens[row, col];
                }
            }

            // Check 4 diagonal down
            for (int row = 0; row < Height - 3; row++)
            {
                for (int col = 0; col < Width - 3; col++)
                {
                    if (IsFourDiagonalDown(row, col)) return Tokens[row, col];
                }
            }

            return '-';

        }

        public bool IsFourUp(int bottomRow, int col)
        {
            if (Tokens[bottomRow, col] == '-') return false;
            return (bottomRow < Height && bottomRow - 3 >= 0 &&
                Tokens[bottomRow, col] == Tokens[bottomRow - 1, col] &&
                Tokens[bottomRow, col] == Tokens[bottomRow - 2, col] &&
                Tokens[bottomRow, col] == Tokens[bottomRow - 3, col]);
        }

        public bool IsFourAcross(int row, int leftCol)
        {
            if (Tokens[row, leftCol] == '-') return false;
            return (leftCol >= 0 && leftCol + 3 < Width &&
                Tokens[row, leftCol] == Tokens[row, leftCol + 1] &&
                Tokens[row, leftCol] == Tokens[row, leftCol + 2] &&
                Tokens[row, leftCol] == Tokens[row, leftCol + 3]);
        }

        public bool IsFourDiagonalUp(int bottomRow, int leftCol)
        {
            if (Tokens[bottomRow, leftCol] == '-') return false;
            return (bottomRow < Height && bottomRow - 3 >= 0 &&
                leftCol >= 0 && leftCol + 3 < Width &&
                Tokens[bottomRow, leftCol] == Tokens[bottomRow - 1, leftCol + 1] &&
                Tokens[bottomRow, leftCol] == Tokens[bottomRow - 2, leftCol + 2] &&
                Tokens[bottomRow, leftCol] == Tokens[bottomRow - 3, leftCol + 3]);
        }

        public bool IsFourDiagonalDown(int topRow, int leftCol)
        {
            if (Tokens[topRow, leftCol] == '-') return false;
            return (topRow >= 0 && topRow + 3 < Height &&
                leftCol >= 0 && leftCol + 3 < Width &&
                Tokens[topRow, leftCol] == Tokens[topRow + 1, leftCol + 1] &&
                Tokens[topRow, leftCol] == Tokens[topRow + 2, leftCol + 2] &&
                Tokens[topRow, leftCol] == Tokens[topRow + 3, leftCol + 3]);
        }

        /// <summary>
        /// Returns a string representation of the board, where each row is broken by \n and each item is separated by ','
        /// </summary>
        /// <returns></returns>
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
