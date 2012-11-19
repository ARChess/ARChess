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
                    Vector3 direction = new Vector3(0, -1, -1);
                    position = Vector3.Transform(position, selectorMat * boardMat_inv);
                    //direction = Vector3.Transform(direction, selectorMat * boardMat_inv);
                    position = position / 31;
                    //direction = direction / 31;
                    //System.Diagnostics.Debug.WriteLine(position);
                    Vector3 boardCoord = (position + direction * position.Z) + new Vector3(8.5f,8.5f,0);
                    

                    int x = (int)(boardCoord.X < 5 ? (4 - boardCoord.X) * -1 : boardCoord.X - 4),
                        y = (int)(boardCoord.Y < 5 ? (4 - boardCoord.Y) * -1 : boardCoord.Y - 4);

                   
                    
                    if ((x >= 0) && (x <= 7) && (y >= 0) && (y <= 7))
                    {
                        //System.Diagnostics.Debug.WriteLine(new Vector2(x, y));
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
                            if (elapsedSpan.TotalSeconds > 2.0)
                            {
                                // Selection has been held 1 second
                                //mSelected = true;
                                selectedSince = DateTime.Now;
                                GameState state = GameState.getInstance();
                                state.setSelected(mPosition);
                            }
                        }
                    }


                    direction = direction * 15;
                    ModelDrawer.DrawLine(mBoardMarker, position, position + direction);
                    //ModelDrawer.DrawLine(mBoardMarker, direction, new Vector3(0, 0, 0));
                    ModelDrawer.Draw(mBoardMarker, ModelSelector.getModel(ModelSelector.Pieces.RED_SQUARE), x, y, 0.2);
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
