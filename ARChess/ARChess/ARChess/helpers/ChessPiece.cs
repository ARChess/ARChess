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

        public ChessPiece(ContentManager _content, Color _player, Piece _type, Vector2 _position)
        {
            mType = _type;
            mPlayer = _player;
            content = _content;

            switch(_type)
            {
                case Piece.PAWN :
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_PAWN : ModelSelector.Pieces.BLACK_PAWN);
                    break;
                case Piece.ROOK:
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_ROOK : ModelSelector.Pieces.BLACK_ROOK);
                    break;
                case Piece.KNIGHT:
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_KNIGHT : ModelSelector.Pieces.BLACK_KNIGHT);
                    break;
                case Piece.BISHOP:
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_BISHOP : ModelSelector.Pieces.BLACK_BISHOP);
                    break;
                case Piece.QUEEN:
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_QUEEN : ModelSelector.Pieces.BLACK_QUEEN);
                    break;
                case Piece.KING:
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

        public void setPosition(Vector2 _position)
        {
            mPosition = _position;

            // Constrain position to board
            if (mPosition.X < 0) { mPosition.X = 0; }
            else if (mPosition.X > 7) { mPosition.X = 7; }
            if (mPosition.Y < 0) { mPosition.Y = 0; }
            else if (mPosition.Y > 7) { mPosition.Y = 7; }
        }

        public void Draw(DetectionResult markerResult)
        {
            if(getType() == ChessPiece.Piece.KNIGHT)
            {
                if(getPlayer() == ChessPiece.Color.BLACK)
                {
                    ModelDrawer.Draw(markerResult, mModel, (int)mPosition.X, (int)mPosition.Y, .2, -1.5707f); //rotated by -90 degress
                }
                else
                {
                    ModelDrawer.Draw(markerResult, mModel, (int)mPosition.X, (int)mPosition.Y, .2, 1.5707f); //rotated by 90 degress
                }
            }
            else
            {
                ModelDrawer.Draw(markerResult, mModel, (int)mPosition.X, (int)mPosition.Y, .2);
            }
        }

        public String toString()
        {
            String pieceString = "";

            return pieceString;
        }

    }
}
