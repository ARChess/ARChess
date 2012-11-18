using System;
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

namespace ARChess
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
            return Marker.LoadFromResource("resources/selection_marker.pat", 16, 16, 40, "selection_marker");
        }

        /*
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
         * */

        public DetectionResult getDetectionResult()
        {
            return mDetectionResult;
        }

        public void setDetectionResult(DetectionResult result)
        {
            mDetectionResult = result;
        }

        public void setBoardMarker(DetectionResult res)
        {
            mBoardMarker = res;
        }


        private DateTime selectedSince;
        public void Draw()
        {
            if (mDetectionResult != null)
            {
                if ((mBoardMarker != null))
                {
                    Microsoft.Xna.Framework.Matrix boardMat = mBoardMarker.Transformation.ToXnaMatrix(),
                        selectorMat = mDetectionResult.Transformation.ToXnaMatrix(),
                        boardMat_inv;

                    boardMat_inv = Microsoft.Xna.Framework.Matrix.Invert(boardMat);

                    Vector3 position = new Vector3(0, 0, 0);
                    Vector3 direction = new Vector3(0, 100, 0);
                    position = Vector3.Transform(position, selectorMat * boardMat_inv);
                    direction = Vector3.Transform(direction, selectorMat * boardMat_inv);
                    Vector3 boardCoord = direction / direction.Z * position.Z;
                    boardCoord = boardCoord / 31;

                    int x = (int)(boardCoord.X),
                        y = (int)(boardCoord.Y);

                    System.Diagnostics.Debug.WriteLine(boardCoord);
                    
                    if ((x >= 0) && (x <= 7) && (y >= 0) && (y <= 7))
                    {
                        // Selection is on board
                        if (mPosition != new Vector2(x, y))
                        {
                            // Selection has changed
                            selectedSince = DateTime.Now;
                            mPosition.X = x;
                            mPosition.Y = y;
                            mSelected = false;
                        }
                        else
                        {
                            // Selection has been held
                            DateTime time = DateTime.Now;
                            long elapsedTicks = time.Ticks - selectedSince.Ticks;
                            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                            if (elapsedSpan.TotalSeconds > 3.0)
                            {
                                // Selection has been held 3 seconds
                                //mSelected = true;
                                GameState state = GameState.getInstance();
                                state.setSelected(mPosition);
                            }
                        }
                    }

                    

                    //ModelDrawer.DrawLine(mDetectionResult, new Vector3(0, 0, 0), position);
                    ModelDrawer.DrawLine(mDetectionResult, new Vector3(1, 0, 0), new Vector3(1, 15, 0));
                    //ModelDrawer.DrawLine(mBoardMarker, position, position + direction);
                    //ModelDrawer.Draw(mBoardMarker, ModelSelector.getModel(ModelSelector.Pieces.RED_SQUARE), mPosition.X, mPosition.Y, 0.2);
                }
                else
                {
                   
                    ModelDrawer.DrawLine(mDetectionResult, new Vector3(0, 0, 0), new Vector3(0, 20, 0));
                }
            }
            else
            {
               // mSelected = false;
            }
        }

    }
}
