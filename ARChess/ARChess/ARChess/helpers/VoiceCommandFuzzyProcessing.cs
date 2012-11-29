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

            if (command.ToLower().IndexOf("move") != -1)
            {
                //find space identity
                r3 = new Regex(@"space ([A-F1-8]) ([A-Za-z0-9\-]+)");

                match3 = r3.Match(command);
            }
            else
            {
                //find piece after select or select the
                r1 = new Regex(@"select ([A-Za-z0-9\-]+)");
                r2 = new Regex(@"select the ([A-Za-z0-9\-]+)");

                match1 = r1.Match(command);
                match2 = r2.Match(command);

                //find space identity
                r3 = new Regex(@"space ([A-F1-8]) ([A-Za-z0-9\-]+)");
               
                match3 = r3.Match(command);
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


                chosenLocation = processLocation(match3.Groups[1].Value, match3.Groups[2].Value);

                Vector2 closestApproximation = findClosestPiece(chosenLocation, chosenPiece);
                GameState.getInstance().setSelected(closestApproximation);
            }
            else
            {
                Vector2 chosenLocation = processLocation(match3.Groups[1].Value, match3.Groups[2].Value);

                GameState.getInstance().setSelected(chosenLocation);
            }
        }

        private static Vector2 processLocation(string location_match, string location_match_alternate)
        {
            Vector2 chosenLocation = new Vector2(4,4); //default to approximately center
            
            switch (location_match)
            {
                case "A": chosenLocation.Y = 1; break;
                case "B": chosenLocation.Y = 2; break;
                case "C": chosenLocation.Y = 3; break;
                case "D": chosenLocation.Y = 4; break;
                case "E": chosenLocation.Y = 5; break;
                case "F": chosenLocation.Y = 6; break;
                case "G": chosenLocation.Y = 7; break;
                case "H": chosenLocation.Y = 8; break;
            }

            switch (location_match_alternate)
            {
                case "one": chosenLocation.Y = 1; break;
                case "two": chosenLocation.Y = 2; break;
                case "three": chosenLocation.Y = 3; break;
                case "four": chosenLocation.Y = 4; break;
                case "five": chosenLocation.Y = 5; break;
                case "six": chosenLocation.Y = 6; break;
                case "seven": chosenLocation.Y = 7; break;
                case "eight": chosenLocation.Y = 8; break;
            }
            
            return chosenLocation;
        }

        private static ChessPiece.Piece processPiece(string piece_name)
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

        private static Vector2 findClosestPiece(Vector2 chosenLocation, ChessPiece.Piece chosenPiece)
        {
            Dictionary<string, ChessPiece> chessPieces = GameState.getInstance().getPieces();
            string color = GameStateManager.getInstance().getCurrentPlayer().ToString().ToLower() + "_";
            Vector2 closestLocation = new Vector2();

            switch (chosenPiece.ToString().ToLower())
            {
                case "rook":
                    for (int i = 1; i <= 2; ++i)
                    {
                        if (Vector2.Distance(chosenLocation, chessPieces[color + chosenPiece.ToString().ToLower() + i].getPosition()) < Vector2.Distance(chosenLocation, closestLocation))
                        {
                            closestLocation = chessPieces[color + chosenPiece.ToString().ToLower() + i].getPosition();
                        }
                    }
                    break;
                case "bishop":
                    for (int i = 1; i <= 2; ++i)
                    {
                        if (Vector2.Distance(chosenLocation, chessPieces[color + chosenPiece.ToString().ToLower() + i].getPosition()) < Vector2.Distance(chosenLocation, closestLocation))
                        {
                            closestLocation = chessPieces[color + chosenPiece.ToString().ToLower() + i].getPosition();
                        }
                    }
                    break;
                case "knight":
                    for (int i = 1; i <= 2; ++i)
                    {
                        if (Vector2.Distance(chosenLocation, chessPieces[color + chosenPiece.ToString().ToLower() + i].getPosition()) < Vector2.Distance(chosenLocation, closestLocation))
                        {
                            closestLocation = chessPieces[color + chosenPiece.ToString().ToLower() + i].getPosition();
                        }
                    }
                    break;
                case "king":
                    closestLocation = chessPieces[color + chosenPiece.ToString().ToLower()].getPosition();
                    break;
                case "queen":
                    closestLocation = chessPieces[color + chosenPiece.ToString().ToLower()].getPosition();
                    break;
                case "pawn":
                    for (int i = 1; i <= 8; ++i)
                    {
                        if (Vector2.Distance(chosenLocation, chessPieces[color + chosenPiece.ToString().ToLower() + i].getPosition()) < Vector2.Distance(chosenLocation, closestLocation))
                        {
                            closestLocation = chessPieces[color + chosenPiece.ToString().ToLower() + i].getPosition();
                        }
                    }
                    break;
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
