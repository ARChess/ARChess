using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Devices;
using SLARToolKit;
using System.Windows.Threading;
using System.Windows.Media.Media3D;

namespace ARChess
{
    public class ModelSelector
    {
        public enum Pieces { LIGHT_SQUARE, DARK_SQUARE, RED_SQUARE, BLUE_SQUARE, GREEN_SQUARE,
                             BLACK_PAWN, WHITE_PAWN, BLACK_ROOK, WHITE_ROOK, BLACK_KNIGHT, WHITE_KNIGHT,
                             BLACK_BISHOP, WHITE_BISHOP, BLACK_KING, WHITE_KING, BLACK_QUEEN, WHITE_QUEEN };

        private static Dictionary<Pieces, Model> models;

        private static void initializeModels()
        {
            models = new Dictionary<Pieces, Model>();
            ContentManager content = (Application.Current as App).Content;

            models[Pieces.LIGHT_SQUARE] = content.Load<Model>("light_cube");
            models[Pieces.DARK_SQUARE]  = content.Load<Model>("dark_cube");
            models[Pieces.RED_SQUARE]   = content.Load<Model>("red_cube");
            models[Pieces.BLUE_SQUARE]  = content.Load<Model>("blue_cube");
            models[Pieces.GREEN_SQUARE] = content.Load<Model>("green_cube");
            models[Pieces.WHITE_KING]   = content.Load<Model>("king_white");
            models[Pieces.BLACK_KING]   = content.Load<Model>("king_black");
            models[Pieces.WHITE_KNIGHT] = content.Load<Model>("knight_white");
            models[Pieces.BLACK_KNIGHT] = content.Load<Model>("knight_black");
            models[Pieces.WHITE_BISHOP] = content.Load<Model>("bishop_white");
            models[Pieces.BLACK_BISHOP] = content.Load<Model>("bishop_black");
            models[Pieces.WHITE_PAWN]   = content.Load<Model>("pawn_white");
            models[Pieces.BLACK_PAWN]   = content.Load<Model>("pawn_black");
        }

        public static Model getModel(Pieces piece)
        {
            if (models == null)
            {
                initializeModels();
            }

            return models[piece];
        }
    }
}
