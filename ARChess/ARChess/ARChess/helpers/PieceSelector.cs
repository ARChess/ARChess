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
                if ((mBoardMarker != null) && !mSelected)
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
                    if ( mPosition != new Vector2(x,y))
                    {
                        selectedSince = DateTime.Now;
                        mPosition.X = x;
                        mPosition.Y = y;
                        mSelected = false;
                    }
                    else
                    {
                        DateTime time = DateTime.Now;
                        long elapsedTicks = time.Ticks - selectedSince.Ticks;
                        TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                        if (elapsedSpan.TotalSeconds > 3.0)
                        {
                            mSelected = true;
                            GameState state = GameState.getInstance();
                            state.setSelected(mPosition);
                        }
                    }
                    

                    ModelDrawer.Draw(mBoardMarker, ModelSelector.getModel(ModelSelector.Pieces.RED_SQUARE), mPosition.X, mPosition.Y, 0.2);
                }
                else
                {
                    //ModelDrawer.Draw(mDetectionResult, ModelSelector.getModel(ModelSelector.Pieces.RED_SQUARE), 3, 3, 0);
                    VertexPositionColor[] primitiveList = new VertexPositionColor[2];
                    primitiveList[0] = new VertexPositionColor(new Vector3(0, 0, 0), Microsoft.Xna.Framework.Color.Green);
                    primitiveList[1] = new VertexPositionColor(new Vector3(0, 100, 0), Microsoft.Xna.Framework.Color.Green);
                    short[] lineIndices = new short[2]{ 0, 1 };

                    /*
                    SharedGraphicsDeviceManager.Current.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                        PrimitiveType.LineStrip,
                        primitiveList,
                        0,
                        2,
                        lineIndices,
                        0,
                        1
                        );
                     * */
                    ModelDrawer.DrawLine(mDetectionResult, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
                }
            }
            else
            {
                mSelected = false;
            }
        }

    }
}
