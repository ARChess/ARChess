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
        private Vector3 mPosition;
        private ContentManager content;

        public ChessPiece(ContentManager _content, Color _player, Piece _type, Vector3 _position)
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

        public Vector3 getPosition()
        {
            return mPosition;
        }

        public void setPosition(Vector3 _position)
        {
            mPosition = _position;
        }

        public void Draw(DetectionResult markerResult)
        {
            ModelDrawer.Draw(markerResult, mModel, (int)mPosition.X, (int)mPosition.Y, .5);
        }

        public String toString()
        {
            String pieceString = "";

            return pieceString;
        }

    }
}
