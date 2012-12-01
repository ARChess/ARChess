using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
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
            Regex r1 = null, r2 = null, r3 = null;
            Match match1 = null, match2 = null, match3 = null;

            //grab only the parts of the commands that we care about
            if (command.ToLower().IndexOf("move") != -1)
            {
                //find space identity
                r3 = new Regex(@"space ([A-H1-8]) ([A-Za-z0-9\-]+)");

                match3 = r3.Match(command);
            }
            else if (command.ToLower().IndexOf("select") != -1)
            {
                //find piece after select or select the
                r1 = new Regex(@"select ([A-Za-z0-9\-]+)");
                r2 = new Regex(@"select the ([A-Za-z0-9\-]+)");

                match1 = r1.Match(command);
                match2 = r2.Match(command);

                //find space identity
                r3 = new Regex(@"space ([A-H1-8]) ([A-Za-z0-9\-]+)");

                match3 = r3.Match(command);
            }
            else
            {
                throw new Exception(command);
            }

            if (r1 != null && r2 != null)
            {
                GameState.getInstance().resetTurn();
                ChessPiece.Piece chosenPiece;
                Vector2 chosenLocation;

                if (match2.Length == 0)
                {
                    chosenPiece = processPiece(match1.Groups[1].Value);
                }
                else
                {
                    chosenPiece = processPiece(match2.Groups[1].Value);
                }

                //convert A7 or whatever board space to X, Y coordinates
                chosenLocation = processLocation(match3.Groups[1].Value, match3.Groups[2].Value);

                //find the closest piece of the specified type to the specified board square
                Vector2 closestApproximation = findClosestPiece(chosenLocation, chosenPiece);
                GameState.getInstance().setSelected(closestApproximation);
            }
            else
            {
                //convert A7 or whatever board space to X, Y coordinates
                Vector2 chosenLocation = processLocation(match3.Groups[1].Value, match3.Groups[2].Value);

                GameState.getInstance().setSelected(chosenLocation);
            }
        }

        private static Vector2 processLocation(string location_match, string location_match_alternate)
        {
            Vector2 chosenLocation = new Vector2(0,0); //default to approximately center
            
            switch (location_match)
            {
                case "A": chosenLocation.X = 0; break;
                case "B": chosenLocation.X = 1; break;
                case "C": chosenLocation.X = 2; break;
                case "D": chosenLocation.X = 3; break;
                case "E": chosenLocation.X = 4; break;
                case "F": chosenLocation.X = 5; break;
                case "G": chosenLocation.X = 6; break;
                case "H": chosenLocation.X = 7; break;
            }

            switch (location_match_alternate)
            {
                case "one": chosenLocation.Y = 7; break;
                case "two": chosenLocation.Y = 6; break;
                case "three": chosenLocation.Y = 5; break;
                case "four": chosenLocation.Y = 4; break;
                case "five": chosenLocation.Y = 3; break;
                case "six": chosenLocation.Y = 2; break;
                case "seven": chosenLocation.Y = 1; break;
                case "eight": chosenLocation.Y = 0; break;
            }
            
            return chosenLocation;
        }

        private static ChessPiece.Piece processPiece(string piece_name)
        {
            string cur_string = "";
            for (int i = 0; i < piece_name.Length; ++i)
            {
                cur_string += piece_name[i];

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

        private static Vector2 findClosestPiece(Vector2 chosenLocation, ChessPiece.Piece chosenPiece)
        {
            Dictionary<string, ChessPiece> chessPieces = GameState.getInstance().getPieces();
            string color = GameStateManager.getInstance().getCurrentPlayer().ToString().ToLower() + "_";
            Vector2 closestLocation = new Vector2();

            for (int i = 1; i <= 8; ++i)
            {
                if (chessPieces[color + "pawn" + i].getMasqueradeType() == chosenPiece.ToString().ToLower())
                {
                    if (Vector2.Distance(chosenLocation, chessPieces[color + "pawn" + i].getPosition()) < Vector2.Distance(chosenLocation, closestLocation))
                    {
                        closestLocation = chessPieces[color + "pawn" + i].getPosition();
                    }
                }
            }
            for (int i = 1; i <= 2; ++i)
            {
                if (chessPieces[color + "rook" + i].getMasqueradeType() == chosenPiece.ToString().ToLower())
                {
                    if (Vector2.Distance(chosenLocation, chessPieces[color + "rook" + i].getPosition()) < Vector2.Distance(chosenLocation, closestLocation))
                    {
                        closestLocation = chessPieces[color + "rook" + i].getPosition();
                    }
                }
            }
            for (int i = 1; i <= 2; ++i)
            {
                if (chessPieces[color + "knight" + i].getMasqueradeType() == chosenPiece.ToString().ToLower())
                {
                    if (Vector2.Distance(chosenLocation, chessPieces[color + "knight" + i].getPosition()) < Vector2.Distance(chosenLocation, closestLocation))
                    {
                        closestLocation = chessPieces[color + "knight" + i].getPosition();
                    }
                }
            }
            if (chessPieces[color + "king"].getMasqueradeType() == chosenPiece.ToString().ToLower())
            {
                if (Vector2.Distance(chosenLocation, chessPieces[color + "king"].getPosition()) < Vector2.Distance(chosenLocation, closestLocation))
                {
                    closestLocation = chessPieces[color + "king"].getPosition();   
                }
            }
            if (chessPieces[color + "queen"].getMasqueradeType() == chosenPiece.ToString().ToLower())
            {
                if (Vector2.Distance(chosenLocation, chessPieces[color + "queen"].getPosition()) < Vector2.Distance(chosenLocation, closestLocation))
                {
                    closestLocation = chessPieces[color + "queen"].getPosition();
                }
            }
            for (int i = 1; i <= 2; ++i)
            {
                if (chessPieces[color + "bishop" + i].getMasqueradeType() == chosenPiece.ToString().ToLower())
                {
                    if (Vector2.Distance(chosenLocation, chessPieces[color + "bishop" + i].getPosition()) < Vector2.Distance(chosenLocation, closestLocation))
                    {
                        closestLocation = chessPieces[color + "bishop" + i].getPosition();
                    }
                }
            }
            
            return closestLocation;
        }

        private static bool isNumeric(string stringToTest)
        {
            int result;
            return int.TryParse(stringToTest, out result);
        }
    }
}
