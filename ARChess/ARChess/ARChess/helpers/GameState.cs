﻿using System;
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
        private ChessPiece mSelectedPiece;
        private ContentManager content;

        public GameState()
        {
            mChessPieces = new List<ChessPiece>();

            // Initialize all ChessPieces for ech player
            // Start with White team
            initializePlayerPieces(ChessPiece.Color.WHITE);
            initializePlayerPieces(ChessPiece.Color.BLACK);
        }

        public GameState(String _stateString)
        {
            
        }

        private void initializePlayerPieces(ChessPiece.Color _player)
        {
            Vector2 position;
            // Initialize Pawns
            for (int i = 0; i < 8; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 6 : 1), i);
                mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.Piece.PAWN, position));
            }

            // Initialize Knights
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (i == 0 ? 1 : 6));
                mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.Piece.KNIGHT, position));
            }

            position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (_player == ChessPiece.Color.BLACK ? 4 : 3));
            mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.Piece.KING, position));
        }

        public void setSelected(Vector2 position)
        {
            foreach(ChessPiece chessPiece in mChessPieces)
            {
                if (position == chessPiece.getPosition())
                {
                    // Set Selected Piece
                    mSelectedPiece = chessPiece;
                    return;
                }
            }
            // If selected piece not found then deselect
            mSelectedPiece = null;
        }

        public ChessPiece getSelectedPiece()
        {
            return mSelectedPiece;
        }

        public void movePiece(Vector2 dest)
        {
            if (mSelectedPiece != null)
            {
                mSelectedPiece.setPosition(dest);
            }
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
