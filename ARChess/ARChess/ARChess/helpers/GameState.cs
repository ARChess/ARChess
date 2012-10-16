using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using SLARToolKit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ARChess.helpers
{
    public class GameState
    {
        private int scale = 31;
        private ContentManager content;
        private int mSelectedPiece = -1;
        private List<ChessPiece> mChessPieces;

        public GameState()
        {
            Vector2 position;

            position = new Vector2(0,1);
            
            // Initialize all ChessPieces for ech player
            // Start with White team
            //initializePlayerPieces(ChessPiece.WHITE);
            //initializePlayerPieces(ChessPiece.BLACK);
            mChessPieces.Add(new ChessPiece(content, ChessPiece.WHITE, ChessPiece.PAWN, new Vector2(0, 1)));
        }

        public GameState(String _stateString)
        {
            
        }

        private void initializePlayerPieces(int _player)
        {
            Vector2 position;
            // Initialize Pawns
            for (int i = 0; i < 8; i++)
            {
                position = new Vector2(i, 1);
                mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.PAWN, position));
            }

            // Initialize Rooks
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2(i * 7, 0);
                mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.PAWN, position));
            }

            // Initialize Knights
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2(i * 8, 0);
                mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.PAWN, position));
            }
            
        }

        public void loadState(String _stateString)
        {

        }

        public void Draw(SharedGraphicsDeviceManager graphics, DetectionResult markerResult)
        {
            foreach (ChessPiece chessPiece in mChessPieces)
            {
                chessPiece.Draw(graphics ,markerResult);
            }
        }

        public String toString()
        {
            String stateString = "";
            //stateString = mChessPieces.ToString();
            return stateString;
        }

    }
}
