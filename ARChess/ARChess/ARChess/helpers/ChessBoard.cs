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
        public enum BoardSquare { OPEN, ENEMY, FRIEND, CAN_MOVE, CAN_TAKE, POTENTIAL_MOVE};

        private ContentManager content;
        private DetectionResult mBoardMarker;

        private BoardSquare[,] mBoardSquares;

        public ChessBoard(ContentManager _content) 
        {
            content = _content;
            mBoardSquares = new ChessBoard.BoardSquare[8, 8];
            clearBoardSquares();
        }

        public Marker getMarker()
        {
            return Marker.LoadFromResource("resources/marker.pat", 16, 16, 80, "board_marker");
        }

        private Model selectModel(int curX, int curY)
        {
            Model lightCube = ModelSelector.getModel(ModelSelector.Pieces.LIGHT_SQUARE);
            Model darkCube = ModelSelector.getModel(ModelSelector.Pieces.DARK_SQUARE);
            Model greenSquare = ModelSelector.getModel(ModelSelector.Pieces.GREEN_SQUARE);
            Model redSquare = ModelSelector.getModel(ModelSelector.Pieces.RED_SQUARE);

            switch ( mBoardSquares[curX,curY] )
            {
                case BoardSquare.CAN_TAKE :
                    return redSquare;

                case BoardSquare.CAN_MOVE :
                    return greenSquare;

                default :
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

            
        }

        public BoardSquare[,] getBoardSquares()
        {
            return mBoardSquares;
        }

        public void setMoves(BoardSquare[,] currentMoves) {
            mBoardSquares = currentMoves;
        }

        public void clearBoardSquares()
        {
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    mBoardSquares[i, j] = BoardSquare.OPEN;
                }
            }
        }

        public DetectionResult getDetectionResult()
        {
            return mBoardMarker;
        }

        public void setDetectionResult(DetectionResult result)
        {
            mBoardMarker = result;
        }

        /// <summary>
        /// Method the draws out model
        /// </summary>
        /// <param name="graphics"></param>
        public void Draw()
        {
            if (mBoardMarker != null) //Only draw the model if the marker is found
            {
                for (int i = 0; i < 8; ++i)
                {
                    for (int j = 0; j < 8; ++j)
                    {
                        ModelDrawer.Draw(mBoardMarker, selectModel(i, j), i, j, 0);
                    }
                }
            }
        }
    }
}
