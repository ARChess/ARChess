﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SLARToolKit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ARChess.helpers
{
    public class PieceSelector
    {
        private ContentManager content;
        private DetectionResult mDetectionResult;
        private DetectionResult mBoardMarker;

        private bool mSelected = false;
        private Vector2 mPosition;

        public PieceSelector()
        {

        }

        public Marker getMarker()
        {
            return Marker.LoadFromResource("resources/selection_marker.pat", 16, 16, 80, "selection_marker");
        }

        public Vector2 getSelected()
        {
            if (mSelected)
            {
                return mPosition;
            }
            else
            {
                // This is essentially returning null because it will never match a piece position
                return new Vector2(-1,-1);
            }
        }

        public void setDetectionResult(DetectionResult result)
        {
            mDetectionResult = result;
        }

        public void setBoardMarker(DetectionResult res)
        {
            mBoardMarker = res;
        }

        public void Draw()
        {
            if (mDetectionResult != null )
            {
                if (mBoardMarker != null)
                {
                    Microsoft.Xna.Framework.Matrix boardMat = mBoardMarker.Transformation.ToXnaMatrix(),
                        selectorMat = mDetectionResult.Transformation.ToXnaMatrix(),
                        boardMat_inv;

                    boardMat_inv = Microsoft.Xna.Framework.Matrix.Invert(boardMat);

                    Vector3 position = new Vector3(0, 0, 0);
                    position = Vector3.Transform(position, selectorMat * boardMat_inv);

                    //
                    int x = (int) (position.X / 15),
                        y = (int) (position.Y / 15);
                   
                    
                    if (x < 0) { x = 0; }
                    if (x > 7) { x = 7; }
                    if (y < 0) { y = 0; }
                    if (y > 7) { y = 0; }


                    // Check if selected
                    mPosition.X = x;
                    mPosition.Y = y;
                    mSelected = true;
                    

                    ModelDrawer.Draw(mBoardMarker, ModelSelector.getModel(ModelSelector.Pieces.RED_SQUARE), mPosition.X, mPosition.Y, 0.2);
                }
                else
                {
                    ModelDrawer.Draw(mDetectionResult, ModelSelector.getModel(ModelSelector.Pieces.RED_SQUARE), 3, 3, 0);
                }
            }
        }

    }
}
