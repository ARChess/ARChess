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
        private List<ChessPiece> mChessPieces;
        private ContentManager content;

        public GameState(ContentManager _content)
        {
            content = _content;
            mChessPieces = new List<ChessPiece>();

            // Initialize all ChessPieces for ech player
            // Start with White team
            initializePlayerPieces(ChessPiece.Color.WHITE);
            //initializePlayerPieces(ChessPiece.Color.BLACK);
        }

        public GameState(String _stateString)
        {
            
        }

        private void initializePlayerPieces(ChessPiece.Color _player)
        {
            Vector2 position;
            // Initialize Pawns
            /*for (int i = 0; i < 8; i++)
            {
                position = new Vector2(i, 1);
                mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.PAWN, position));
            }

            // Initialize Rooks
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2(i * 7, 0);
                mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.ROOK, position));
            }

            // Initialize Knights
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2((i == 0 ? 1 : 6), 0);
                mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.KNIGHT, position));
            }*/
            
            /*position = new Vector2(0, 4);
            mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.KING, position));*/
        }

        public void loadState(String _stateString)
        {

        }

        public void Draw(DetectionResult markerResult)
        {
            foreach (ChessPiece chessPiece in mChessPieces)
            {
                chessPiece.Draw(markerResult);
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
