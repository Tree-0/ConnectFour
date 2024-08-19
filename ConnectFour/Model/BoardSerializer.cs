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
    /// I don't have the knowledge to set up real multiplayer yet, so turn based exporting / importing codes is
    /// a couple tiers down in terms of quality... good practice.
    /// 
    /// Honestly not sure if there's much point to this class beyond basic algorithm practice.
    /// Ideally, these codes would only need to be stored in N bits where N = binary code string length.
    /// However, I use strings, chars, etc just to work with the data... Is that pointless? 
    /// I'm not really actually working directly with bits.
    /// 
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
                        // This would occur if somehow you have floating tokens 
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

            //// convert to long base 10, then convert to base 16 string
            //ulong decimalCode = Convert.ToUInt64(code.ToString(), 2);
            //string hexCode = decimalCode.ToString("X");

            //Debug.WriteLine($"{code}\n{decimalCode}\n{hexCode}");

            //return hexCode.ToUpper();
        }



        /// <summary>
        /// Return a 2D array representing Board Tokens
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public char[,] DeSerialize(string code, out bool success)
        {

            // if no new code is being imported, just return the current board
            if (code == null || code == string.Empty || code == "code"
                || code == "No code to import")
            {
                success = false;
                return _board.Tokens;
            }


            // convert from 0x to long base 10, then convert base 10 to binary string
            //ulong decimalCode = Convert.ToUInt64(code, 16);
            //string binaryCode = decimalCode.ToString("b");

            string binaryCode = code;

            // I didn't have a good way to properly pad the binaryCode with any needed leading zeros when deserialized 
            // from hexidecimal. i.e. if Board.EmptyRows = 2, binaryCode leads with '10' but I need '010' because the 
            // first 3 bits should always solely be for representing emptyRows...
            //int emptyRows = (_board.Height*_board.Width + _board.Width + 3 - binaryCode.Length)/_board.Width;
            //int emptyRowBits = Convert.ToString(emptyRows, 2).Length;

            int emptyRows = Convert.ToInt32(code.Substring(0, 3), 2);
            int bitsPerColumn = _board.Height - emptyRows + 1;

            // get bits to iterate through, starting after the bits designating EmptyRows
            int bit = 3;

            // Array to return, representing Board
            char[,] Tokens = new char[_board.Height, _board.Width];
            for (int row = 0; row < _board.Height; row++)
            {
                for (int col = 0; col < _board.Width; col++)
                {
                    Tokens[row, col] = '-'; // Initialize all to empty
                }
            }

            // populate Tokens
            for (int col = 0; col < _board.Width; col++)
            {
                bool readingTokens = false;

                for (int row = emptyRows; row < _board.Height; row++)
                {
                    if (bit >= binaryCode.Length) { break; }

                    // empty space in column
                    if (binaryCode[bit] == '0' && !readingTokens)
                    {
                        Tokens[row, col] = '-';
                    }
                    // context switch detected, prepare to place tokens
                    else if (binaryCode[bit] == '1' && !readingTokens)
                    {
                        readingTokens = true;
                        row--;
                    }
                    // red token detected
                    else if (binaryCode[bit] == '0' && readingTokens)
                    {
                        Tokens[row, col] = 'R';

                    }
                    // yellow token detected
                    else if (binaryCode[bit] == '1' && readingTokens)
                    {
                        Tokens[row, col] = 'Y';
                    }

                    // read next 
                    bit++;

                    Debug.WriteLine("Current Deserialization State:");
                    Debug.WriteLine(Print2DArrayState(Tokens, _board.Height, _board.Width));
                    
                }
            }

            success = true;
            return Tokens;
        }

        public void PrintBoardState()
        {
            Debug.WriteLine(_board.ToString());
        }

        /// <summary>
        /// Returns a string representation of the board, where each row is broken by \n and each item is separated by ','
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
