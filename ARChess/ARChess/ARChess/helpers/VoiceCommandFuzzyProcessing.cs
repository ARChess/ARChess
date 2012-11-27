using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ARChess
{
    public class VoiceCommandFuzzyProcessing
    {
        public static void process(string command)
        {
            Regex r1 = null, r2 = null, r3 = null, r4 = null;
            Match match1 = null, match2 = null, match3 = null, match4 = null;

            if (command.ToLower().IndexOf("move") != -1)
            {
                //find space identity
                r3 = new Regex(@"space ([A-F])([1-8])");
                r4 = new Regex(@"space ([A-Za-z0-9\-]+)[ ]?([A-Za-z0-9\-]+)");
                match3 = r1.Match(command);
                match4 = r2.Match(command);
            }
            else
            {
                //find piece after select or select the
                r1 = new Regex(@"select ([A-Za-z0-9\-]+)");
                r2 = new Regex(@"select the ([A-Za-z0-9\-]+)");

                match1 = r1.Match(command);
                match2 = r2.Match(command);

                //find space identity
                r3 = new Regex(@"space ([A-F1-8])([1-8])");
                r4 = new Regex(@"space ([A-Za-z0-9\-]+)[ ]?([A-Za-z0-9\-]+)");
                match3 = r1.Match(command);
                match4 = r2.Match(command);
            }
            if (r1 != null && r2 != null)
            {
                GameState.getInstance().resetTurn();
            }
            else
            {

            }
        }

        private Vector2 processLocation(string location_match, string location_match_alternate = null)
        {
            return Vector2.Zero;   
        }

        private ChessPiece.Piece processPiece(string piece_name)
        {
            string cur_string = "";
            for (int i = 0; i < piece_name.Length; ++i)
            {
                cur_string += piece_name;

                if (cur_string == "ki" || cur_string == "ke")
                {
                    return ChessPiece.Piece.KING;
                }
                else if (cur_string == "kn")
                {
                    return ChessPiece.Piece.KNIGHT;
                }
                else if (cur_string == "q")
                {
                    return ChessPiece.Piece.QUEEN;
                }
                else if (cur_string == "b")
                {
                    return ChessPiece.Piece.BISHOP;
                }
                else if (cur_string == "p")
                {
                    return ChessPiece.Piece.PAWN;
                }
                else if (cur_string == "r")
                {
                    return ChessPiece.Piece.ROOK;
                }
            }
            //default to king
            return ChessPiece.Piece.KING;
        }

        private Vector2 findClosestPiece(Vector2 chosenLocation, ChessPiece.Piece chosenPiece)
        {
            return Vector2.Zero;
        }
    }
}
