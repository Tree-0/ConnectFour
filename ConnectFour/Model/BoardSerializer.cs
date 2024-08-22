using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ConnectFour.Model
{
    /// <summary>
    /// Class for importing and exporting codes that represent the state of the Connect 4 Board. 
    /// </summary>
    public class BoardSerializer
    {
        private Board _board;

        public int Code { get; }

        public BoardSerializer(Board board)
        {
            _board = board;
        }

        /// <summary>
        /// Return a string representing the binary state of _board
        ///
        /// </summary>
        /// <returns></returns>
        /// 
        public string Serialize()
        {
            // 3 bits at the beginning specify the number of empty rows
            int emptyRows = _board.EmptyRows;

            // Each column can be represented by:
            // -    excluding a bit for each empty row
            // -    including a 0 for each remaining empty space in the column
            // -    including a bit with value = 1 to signal a switch to token notation
            // -    any remaining bits while bits <= bitsPerColumn are 0 = 'R' 1 = 'Y'
            int bitsPerColumn = _board.Height - emptyRows + 1;


            // construct the serialization of the board
            StringBuilder code = new StringBuilder(Convert.ToString(emptyRows, 2).PadLeft(3, '0'));

            for (int col = 0; col < _board.Width; col++)
            {
                // for each row, place 0 for empty space until tokens detected
                bool placingTokens = false; 

                for (int row = emptyRows; row < _board.Height; row++)
                {
                    char token = _board.Tokens[row, col];
                   
                    if (token == '-' && !placingTokens)
                    {
                        // empty space, add a zero
                        code.Append(0);
                    }
                    else if (token == 'R')
                    {
                        if (!placingTokens)
                        {
                            // first token found, add a 1
                            code.Append(1);
                            placingTokens = true;
                        }

                        // add a zero for Red tokens
                        code.Append(0);
                    }
                    else if (token == 'Y')
                    {
                        if (!placingTokens)
                        {
                            // first token found, add a 1
                            code.Append(1);
                            placingTokens = true;
                        }

                        // add a one for Yellow tokens
                        code.Append(1);
                    }
                    else
                    {
                        // This would occur if there is an empty space '-' found but placingTokens == true
                        // This would imply that there is a floating token.
                        // tokens should always either be at the bottom or affected by gravity
                        throw new Exception("Board is not valid, stopping serialization");
                    }
                }

                // if no tokens were placed in this row, should be all zeroes and a 1 at the very end of the column
                if (!placingTokens)
                {
                    code.Append(1);
                }
                    
            }

            return code.ToString();
        }



        /// <summary>
        /// Return a 2D array representing Board Tokens
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public char[,] DeSerialize(string code, out bool success)
        {
            // if no new or valid code is being imported, just return the current board
            if (code == null || code == string.Empty
                || code == "code"
                || code == "Invalid code"
                || !ContainsOnlyAllowedChars(code, "10"))
            {
                success = false;
                return _board.Tokens;
            }

            string binaryCode = code;

            int emptyRows = Convert.ToInt32(code.Substring(0, 3), 2);
            int bitsPerColumn = _board.Height - emptyRows + 1;

            // Array to return, representing Board
            char[,] Tokens = new char[_board.Height, _board.Width];
            for (int row = 0; row < _board.Height; row++)
            {
                for (int col = 0; col < _board.Width; col++)
                {
                    Tokens[row, col] = '-'; // Initialize all to empty
                }
            }

            // populate board with tokens
            
            int bit = 3;  // Start reading after the first 3 bits

            for (int col = 0; col < _board.Width; col++)
            {
                Debug.WriteLine($"Binary: {binaryCode}");
                Debug.WriteLine($"Index: {bit}");
                Debug.WriteLine(Print2DArrayState(Tokens, _board.Height, _board.Width));

                bool readingTokens = false;

                int row = emptyRows;
                while (row <= _board.Height)
                {
                    // reading any empty spaces
                    if (!readingTokens)
                    {
                        if (binaryCode[bit] == '0')
                        {
                            row++;
                            bit++;
                        }
                        // token signifier found, switch to reading tokens
                        else
                        {
                            readingTokens = true;
                            bit++;
                        }
                    }
                    // bottom of column reached
                    else if (row >= _board.Height)
                    {
                        break;
                    }
                    // reading tokens
                    else
                    {
                        if (binaryCode[bit] == '0')
                        {
                            Tokens[row, col] = 'R';
                            row++;
                            bit++;
                        }
                        else
                        {
                            Tokens[row, col] = 'Y';
                            row++;
                            bit++;
                        }
                    }


                }
            }

            success = true;
            return Tokens;
        }


        /// <summary>
        /// Make sure that the input string only contains characters in the allowedChars string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="allowedChars"></param>
        /// <returns></returns>
        public bool ContainsOnlyAllowedChars(string input, string allowedChars)
        {
            var allowedSet = new HashSet<char>(allowedChars);
            return input.All(c => allowedSet.Contains(c));
        }


        public void PrintBoardState()
        {
            Debug.WriteLine(_board.ToString());
        }

        /// <summary>
        /// Returns a string representation of the board, where each row is broken by \n and each item is separated by ','
        /// 
        /// For debugging the DeSerialize function, when a char[,] array is being populated but there is no parent Board object
        /// </summary>
        /// <returns></returns>
        public string Print2DArrayState(char[,] Tokens, int Height, int Width)
        {
            StringBuilder sb = new StringBuilder();

            for (int row = 0; row < Height; row++)
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
