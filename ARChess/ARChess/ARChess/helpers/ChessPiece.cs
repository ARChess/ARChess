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
        public static const int PAWN   = 0;
        public static const int ROOK   = 1;
        public static const int KNIGHT = 2;
        public static const int BISHOP = 3;
        public static const int QUEEN  = 4;
        public static const int KING   = 5;

        private int mType;
        private int mPlayer;
        private Model mModel;
        Vector2 mPosition;
        //Position

        public ChessPiece(ContentManager _content, int _type, Vector2 _position)
        {
            mType = _type;

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
            //mModel = _content.Load<Model>(modelName);
            mPosition = _position;
        }

        public void Draw() {

        }

        public String toString()
        {
            String pieceString = "";

            return pieceString;
        }

    }
}
