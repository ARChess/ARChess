using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using SLARToolKit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ARChess
{
    public class ChessPiece
    {
        public enum Color { WHITE, BLACK };

        public enum Piece { PAWN, ROOK, KNIGHT, BISHOP, QUEEN, KING };

        private Piece mType;
        private Color mPlayer;
        private Model mModel;
        private Vector2 mPosition;
        private ContentManager content;
        private string masqueradesAs;
        private ChessBoard.BoardSquare[,] mMoves;

        public ChessPiece(ContentManager _content, Color _player, Piece _type, Vector2 _position, string _masqueradeType)
        {
            initialize(_content, _player, _type, _position, _masqueradeType);
        }

        private void initialize(ContentManager _content, Color _player, Piece _type, Vector2 _position, string _masqueradeType)
        {
            mType = _type;
            mPlayer = _player;
            content = _content;
            masqueradesAs = _masqueradeType;

            switch (masqueradesAs)
            {
                case "pawn":
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_PAWN : ModelSelector.Pieces.BLACK_PAWN);
                    break;
                case "rook":
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_ROOK : ModelSelector.Pieces.BLACK_ROOK);
                    break;
                case "knight":
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_KNIGHT : ModelSelector.Pieces.BLACK_KNIGHT);
                    break;
                case "bishop":
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_BISHOP : ModelSelector.Pieces.BLACK_BISHOP);
                    break;
                case "queen":
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_QUEEN : ModelSelector.Pieces.BLACK_QUEEN);
                    break;
                case "king":
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_KING : ModelSelector.Pieces.BLACK_KING);
                    break;
            }

            mPosition = _position;
        }

        public Piece getType()
        {
            return mType;
        }

        public Color getPlayer()
        {
            return mPlayer;
        }

        public Model getModel()
        {
            return mModel;
        }

        public Vector2 getPosition()
        {
            return mPosition;
        }

        public string getMasqueradeType()
        {
            return masqueradesAs;
        }

        public void setMasqueradesAs(string type)
        {
            masqueradesAs = type;

            switch (masqueradesAs)
            {
                case "pawn":
                    mModel = ModelSelector.getModel(mPlayer == Color.WHITE ? ModelSelector.Pieces.WHITE_PAWN : ModelSelector.Pieces.BLACK_PAWN);
                    break;
                case "rook":
                    mModel = ModelSelector.getModel(mPlayer == Color.WHITE ? ModelSelector.Pieces.WHITE_ROOK : ModelSelector.Pieces.BLACK_ROOK);
                    break;
                case "knight":
                    mModel = ModelSelector.getModel(mPlayer == Color.WHITE ? ModelSelector.Pieces.WHITE_KNIGHT : ModelSelector.Pieces.BLACK_KNIGHT);
                    break;
                case "bishop":
                    mModel = ModelSelector.getModel(mPlayer == Color.WHITE ? ModelSelector.Pieces.WHITE_BISHOP : ModelSelector.Pieces.BLACK_BISHOP);
                    break;
                case "queen":
                    mModel = ModelSelector.getModel(mPlayer == Color.WHITE ? ModelSelector.Pieces.WHITE_QUEEN : ModelSelector.Pieces.BLACK_QUEEN);
                    break;
                case "king":
                    mModel = ModelSelector.getModel(mPlayer == Color.WHITE ? ModelSelector.Pieces.WHITE_KING : ModelSelector.Pieces.BLACK_KING);
                    break;
            }
        }

        public void remove()
        {
            mPosition = new Vector2(-1, -1);
        }

        public void setPosition(Vector2 _position)
        {
            mPosition = _position;
        }

        public bool isTaken()
        {
            return (mPosition.X == -1) && (mPosition.Y == -1);
        }

        public void determineMoves(Dictionary<string, ChessPiece> chessPieces)
        {
            // Set entire board to OPEN
            mMoves = new ChessBoard.BoardSquare[8, 8];
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    mMoves[i, j] = ChessBoard.BoardSquare.OPEN;
                }
            }
            
            // Load FRIEND and ENEMY positions onto squares
            foreach (KeyValuePair<string, ChessPiece> entry in chessPieces)
            {
                if (!entry.Value.isTaken())
                {
                    Vector2 pos = entry.Value.getPosition();
                    if (entry.Value.getPlayer() == mPlayer)
                    {
                        mMoves[(int)pos.X, (int)pos.Y] = ChessBoard.BoardSquare.FRIEND;
                    }
                    else
                    {
                        mMoves[(int)pos.X, (int)pos.Y] = ChessBoard.BoardSquare.ENEMY;
                    }
                }
            }

            // Based on Type, determine potential moves
            int forward = (mPlayer == ChessPiece.Color.WHITE ? 1 : -1);
            int x = (int)mPosition.X;
            int y = (int)mPosition.Y;
            List<Vector2> potentialMoves = new List<Vector2>();

            switch (masqueradesAs)
            {
                case "pawn":
                    // Pawns can only take diagonally
                    if ((y > 0) && (mMoves[x + forward, y - 1] == ChessBoard.BoardSquare.ENEMY))
                    {
                        mMoves[x + forward, y - 1] = ChessBoard.BoardSquare.CAN_TAKE;
                    }
                    if ((y < 7) && (mMoves[x + forward, y + 1] == ChessBoard.BoardSquare.ENEMY))
                    {
                        mMoves[x + forward, y + 1] = ChessBoard.BoardSquare.CAN_TAKE;
                    }

                    potentialMoves.Add(new Vector2(x + forward, y));
                    // Special first move case
                    if (x == (forward > 0 ? 1 : 6))
                    {
                        potentialMoves.Add(new Vector2(x + 2 * forward, y));
                    }
                    break;

                case "rook":
                    slidePiece(new Vector2( 0,  1), 8);
                    slidePiece(new Vector2( 0, -1), 8);
                    slidePiece(new Vector2( 1,  0), 8);
                    slidePiece(new Vector2(-1,  0), 8);
                    break;

                case "knight":
                    for (int i = -1; i <= 1; i = i + 2)
                    {
                        for (int j = -1; j <= 1; j = j + 2)
                        {
                            potentialMoves.Add(new Vector2(x + 2 * i, y + j));
                            potentialMoves.Add(new Vector2(x + i, y + 2 * j));
                        }
                    }
                    break;

                case "bishop":
                    slidePiece(new Vector2( 1,  1), 8);
                    slidePiece(new Vector2(-1,  1), 8);
                    slidePiece(new Vector2( 1, -1), 8);
                    slidePiece(new Vector2(-1, -1), 8);
                    break;

                case "queen":
                    // Essentially a Rook and Bishop combined
                    slidePiece(new Vector2( 0,  1), 8);
                    slidePiece(new Vector2( 0, -1), 8);
                    slidePiece(new Vector2( 1,  0), 8);
                    slidePiece(new Vector2(-1,  0), 8);
                    slidePiece(new Vector2( 1,  1), 8);
                    slidePiece(new Vector2(-1,  1), 8);
                    slidePiece(new Vector2( 1, -1), 8);
                    slidePiece(new Vector2(-1, -1), 8);
                    break;

                case "king":
                    slidePiece(new Vector2( 0,  1), 1);
                    slidePiece(new Vector2( 0, -1), 1);
                    slidePiece(new Vector2( 1,  0), 1);
                    slidePiece(new Vector2(-1,  0), 1);
                    slidePiece(new Vector2( 1,  1), 1);
                    slidePiece(new Vector2(-1,  1), 1);
                    slidePiece(new Vector2( 1, -1), 1);
                    slidePiece(new Vector2(-1, -1), 1);
                    break;
            }

            // Check potential moves
            foreach (Vector2 pos in potentialMoves)
            {
                x = (int)pos.X;
                y = (int)pos.Y;
                if ((x < 8) && (x >= 0) && (y < 8) && (y >= 0))
                {
                    // Potential move is inside board
                    if (mMoves[x, y] == ChessBoard.BoardSquare.ENEMY)
                    {
                        if (mType != ChessPiece.Piece.PAWN)
                        {
                            mMoves[x, y] = ChessBoard.BoardSquare.CAN_TAKE;
                        }
                    }
                    else if (mMoves[x, y] != ChessBoard.BoardSquare.FRIEND)
                    {
                        mMoves[x, y] = ChessBoard.BoardSquare.CAN_MOVE;
                    }
                    else
                    {
                        // Cannot move
                    }
                }
            }
        }

        private void slidePiece(Vector2 direction, int range)
        {
            int x = (int)mPosition.X;
            int y = (int)mPosition.Y;

            int xDelta = (int)direction.X;
            int yDelta = (int)direction.Y;

            for (int j = 1; j <= range; ++j)
            {
                int boardX = x + j * xDelta;
                int boardY = y + j * yDelta;
                if ((boardX > 7) || (boardX < 0) || (boardY > 7) || (boardY < 0))
                {
                    break;
                }
                else if (mMoves[boardX, boardY] == ChessBoard.BoardSquare.FRIEND)
                {
                    //break;
                }
                else if (mMoves[boardX, boardY] == ChessBoard.BoardSquare.ENEMY)
                {
                    mMoves[boardX, boardY] = ChessBoard.BoardSquare.CAN_TAKE;
                    break;
                }
                else
                {
                    mMoves[boardX, boardY] = ChessBoard.BoardSquare.CAN_MOVE;
                }
                mMoves[boardX, boardY] = ChessBoard.BoardSquare.CAN_MOVE;
            }
        }

        public ChessBoard.BoardSquare[,] getMoves()
        {
            return mMoves;
        }
       
        public void Draw(DetectionResult markerResult)
        {
            Draw(markerResult, new Vector3((int)mPosition.X, (int)mPosition.Y, 0.2f));
        }

        public void Draw(DetectionResult markerResult, Vector3 position)
        {
            if (!isTaken())
            {
                if (getType() == ChessPiece.Piece.KNIGHT)
                {
                    if (getPlayer() == ChessPiece.Color.BLACK)
                    {
                        ModelDrawer.Draw(markerResult, mModel, (int)position.X, (int)position.Y, .2, -1.5707f); //rotated by -90 degress
                    }
                    else
                    {
                        ModelDrawer.Draw(markerResult, mModel, (int)position.X, (int)position.Y, .2, 1.5707f); //rotated by 90 degress
                    }
                }
                else if (getType() == ChessPiece.Piece.BISHOP)
                {
                    if (getPlayer() == ChessPiece.Color.BLACK)
                    {
                        ModelDrawer.Draw(markerResult, mModel, (int)position.X, (int)position.Y, .2, -3.1414f); //rotated by -90 degress
                    }
                    else
                    {
                        ModelDrawer.Draw(markerResult, mModel, (int)position.X, (int)position.Y, .2, 0.0f); //rotated by 90 degress
                    }
                }
                else
                {
                    ModelDrawer.Draw(markerResult, mModel, (int)position.X, (int)position.Y, 0.2);
                }
            }
        }

        public void DrawAtLocation(DetectionResult markerResult, Vector2 positionToDrawAt)
        {
            if (getType() == ChessPiece.Piece.KNIGHT)
            {
                if (getPlayer() == ChessPiece.Color.BLACK)
                {
                    ModelDrawer.Draw(markerResult, mModel, positionToDrawAt.X, positionToDrawAt.Y, .2, -1.5707f, true); //rotated by -90 degress
                }
                else
                {
                    ModelDrawer.Draw(markerResult, mModel, positionToDrawAt.X, positionToDrawAt.Y, .2, 1.5707f, true); //rotated by 90 degress
                }
            }
            else if (getType() == ChessPiece.Piece.BISHOP)
            {
                if (getPlayer() == ChessPiece.Color.BLACK)
                {
                    ModelDrawer.Draw(markerResult, mModel, positionToDrawAt.X, positionToDrawAt.Y, .2, -3.1414f, true); //rotated by -90 degress
                }
                else
                {
                    ModelDrawer.Draw(markerResult, mModel, positionToDrawAt.X, positionToDrawAt.Y, .2, 0.0f, true); //rotated by 90 degress
                }
            }
            else
            {
                ModelDrawer.Draw(markerResult, mModel, positionToDrawAt.X, positionToDrawAt.Y, 0.2, 0.0f, true);
            }
        }
    }
}
