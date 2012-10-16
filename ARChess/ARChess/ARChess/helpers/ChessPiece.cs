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

namespace ARChess.helpers
{
    public class ChessPiece
    {
        public const int WHITE = 0;
        public const int BLACK = 1;

        public const int PAWN   = 0;
        public const int ROOK   = 1;
        public const int KNIGHT = 2;
        public const int BISHOP = 3;
        public const int QUEEN  = 4;
        public const int KING   = 5;

        private int mType;
        private int mPlayer;
        private Model mModel;
        Vector2 mPosition;

        public ChessPiece(ContentManager _content, int _player, int _type, Vector2 _position)
        {
            mType = _type;
            mPlayer = _player;

            String modelName = "";
            switch(_type)
            {
                case PAWN :
                    modelName = "pawn_model";
                    break;
                case ROOK :
                    break;
                case KNIGHT :
                    break;
                case BISHOP :
                    break;
                case QUEEN :
                    break;
                case KING :
                    break;
            }

            if (mPlayer == WHITE)
            {
                modelName += "_white";
            }
            else
            {
                modelName += "_black";
            }

            //mModel = _content.Load<Model>(modelName);
            mPosition = _position;
        }

        public int getType()
        {
            return mType;
        }

        public int getPlayer()
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
        }

        public void Draw(SharedGraphicsDeviceManager graphics, DetectionResult markerResult)
        {

        }

        public String toString()
        {
            String pieceString = "";

            return pieceString;
        }

    }
}
