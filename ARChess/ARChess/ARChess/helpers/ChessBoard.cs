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
    public class ChessBoard
    {
        private ContentManager content;

        public ChessBoard(ContentManager _content) 
        {
            content = _content;
        }

        private Model selectModel(int curX, int curY)
        {
            Model lightCube = ModelSelector.getModel(ModelSelector.Pieces.LIGHT_SQUARE);
            Model darkCube = ModelSelector.getModel(ModelSelector.Pieces.DARK_SQUARE);
           

            if (curX % 2 == 0)
            {
                if (curY % 2 == 0)
                {
                    return lightCube;
                }
                else
                {
                    return darkCube;
                }
            }
            else
            {
                if (curY % 2 == 0)
                {
                    return darkCube;
                }
                else
                {
                    return lightCube;
                }
            }
        }

        /// <summary>
        /// Method the draws out model
        /// </summary>
        /// <param name="graphics"></param>
        public void Draw(DetectionResult markerResult)
        {
            if (markerResult != null) //Only draw the model if the marker is found
            {
                for (int i = 0; i < 8; ++i)
                {
                    for (int j = 0; j < 8; ++j)
                    {
                        ModelDrawer.Draw(markerResult, selectModel(i, j), i, j, 0);
                    }
                }
            }
        }
    }
}
