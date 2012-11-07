using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Xml.Linq;
using SLARToolKit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ARChess
{
    public class GameState
    {
        private ChessPiece.Color mMyColor;
        private List<ChessPiece> mChessPieces;
        private ChessPiece mSelectedPiece;
        private ContentManager content;

        private ChessBoard mBoard;
        private PieceSelector mSelector;

        private static GameState mInstance = null;

        public static GameState getInstance()
        {
            if (mInstance == null)
            {
                mInstance = new GameState();
            }
            return mInstance;
        }

        private GameState()
        {
            initialize();
        }

        private GameState(CurrentGameState state)
        {
            initialize();
            loadState(state);
        }

        private void initialize() 
        {
            mChessPieces = new List<ChessPiece>();

            // Initialize all ChessPieces for ech player
            // White team
            initializePlayerPieces(ChessPiece.Color.WHITE);
            // Black team
            initializePlayerPieces(ChessPiece.Color.BLACK);

            mBoard = new ChessBoard(content);
            mSelector = new PieceSelector();
        }

        public void loadState(CurrentGameState state)
        {

        }

        private void initializePlayerPieces(ChessPiece.Color _player)
        {
            Vector2 position;
            // Initialize Pawns
            for (int i = 0; i < 8; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 6 : 1), i);
                mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.Piece.PAWN, position));
            }

            // Initialize Knights
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (i == 0 ? 1 : 6));
                mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.Piece.KNIGHT, position));
            }

            // Initialize Bishops
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (i == 0 ? 2 : 5));
                mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.Piece.BISHOP, position));
            }

            // Initialize Rooks
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (i == 0 ? 0 : 7));
                mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.Piece.ROOK, position));
            }

            position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (_player == ChessPiece.Color.BLACK ? 4 : 3));
            mChessPieces.Add(new ChessPiece(content, _player, ChessPiece.Piece.KING, position));
        }

        public void setSelected(Vector2 position)
        {
            if (mSelectedPiece == null)
            {
                // Select Piece at position
                foreach (ChessPiece chessPiece in mChessPieces)
                {
                    if (position == chessPiece.getPosition())
                    {
                        // Set Selected Piece
                        mSelectedPiece = chessPiece;

                        // Set board squares
                        setBoardSquares();

                        return;
                    }
                }
            }
            else
            {
                // Piece Selected - check if position is valid move for Piece
                ChessBoard.BoardSquare[,] squares = mBoard.getBoardSquares();
                int x = (int) position.X;
                int y = (int) position.Y;
                if ( squares[x, y] == ChessBoard.BoardSquare.CAN_MOVE )
                {
                    mSelectedPiece = null;
                    mBoard.clearBoardSquares();
                }
                else
                {
                    // Give negative feedback
                }
                // if valid move

            }
            
        }

        public ChessPiece getSelectedPiece()
        {
            return mSelectedPiece;
        }

        public void movePiece(Vector2 dest)
        {
            if (mSelectedPiece != null)
            {
                mSelectedPiece.setPosition(dest);
            }
        }

        public Marker[] getMarkers()
        {
            Marker[] markers = { mBoard.getMarker(), mSelector.getMarker() };
            return markers;
        }

        public void setBoardSquares()
        {
            // Set entire board to OPEN
            ChessBoard.BoardSquare[,] boardSquares = mBoard.getBoardSquares();

            // Selected Piece Details
            //ChessPiece.Color selectedPlayer   = mSelectedPiece.getPlayer();
            ChessPiece.Piece selectedType     = mSelectedPiece.getType();
            Vector2          selectedPosition = mSelectedPiece.getPosition();

            // Load FRIEND and ENEMY positions onto squares
            foreach (ChessPiece piece in mChessPieces)
            {
                Vector2 pos = piece.getPosition();
                if (piece.getPlayer() == mMyColor)
                {
                    boardSquares[(int) pos.X, (int) pos.Y] = ChessBoard.BoardSquare.FRIEND;
                }
                else
                {
                    boardSquares[(int)pos.X, (int)pos.Y] = ChessBoard.BoardSquare.ENEMY;
                }
            }

            // Based on Type, determine potential moves
            int forward = (mMyColor == ChessPiece.Color.WHITE ? 1 : -1);
            int x = (int) selectedPosition.X;
            int y = (int) selectedPosition.Y;
            List<Vector2> potentialMoves = new List<Vector2>();
            List<Vector2> slideDirection = new List<Vector2>();

            switch (selectedType)
            {
                case ChessPiece.Piece.PAWN :
                    potentialMoves.Add( new Vector2(x + forward, y - 1) );
                    potentialMoves.Add( new Vector2(x + forward, y) );
                    potentialMoves.Add( new Vector2(x + forward, y + 1) );
                    break;

                case ChessPiece.Piece.ROOK:
                    slideDirection.Add( new Vector2( 0 ,  1) );
                    slideDirection.Add( new Vector2( 0 , -1) );
                    slideDirection.Add( new Vector2( 1 ,  0) );
                    slideDirection.Add( new Vector2(-1 ,  0) );

                    if (selectedType != ChessPiece.Piece.ROOK)
                    {
                        // QUEEN is a Rook and Bishop combined
                        goto case ChessPiece.Piece.BISHOP;
                    }
                    break;

                case ChessPiece.Piece.KNIGHT:
                    // TODO
                    break;

                case ChessPiece.Piece.BISHOP:
                    slideDirection.Add( new Vector2(1,1) );
                    slideDirection.Add( new Vector2(-1,1) );
                    slideDirection.Add( new Vector2(1,-1) );
                    slideDirection.Add( new Vector2(-1,-1) );  
                    break;

                case ChessPiece.Piece.QUEEN:
                    // Essentially a Rook and Bishop combined
                    goto case ChessPiece.Piece.ROOK;

                case ChessPiece.Piece.KING:
                    for (int i = -1; i < 1; ++i)
                    {
                        potentialMoves.Add( new Vector2(x + i, y - 1) );
                        if (i != 0)
                        {
                            potentialMoves.Add( new Vector2(x + i, y) );
                        }
                        potentialMoves.Add(new Vector2(x + i, y + 1));
                    }
                    break;
            }

            // Move Sliding pieces
            foreach (Vector2 dir in slideDirection)
            {
                int xDelta = (int)dir.X;
                int yDelta = (int)dir.Y;
                for (int j = 0; j < 8; ++j)
                {
                    int boardX = x + j * xDelta;
                    int boardY = y + j * yDelta;
                    if ((boardX > 7) || (boardX < 0) || (boardY > 7) || (boardY < 0))
                    {
                        break;
                    }

                    if (boardSquares[boardX, boardY] == ChessBoard.BoardSquare.FRIEND)
                    {
                        //break;
                    }
                    potentialMoves.Add(new Vector2(boardX, boardY));
                    if (boardSquares[boardX, boardY] == ChessBoard.BoardSquare.ENEMY)
                    {
                        break;
                    }
                }
            }

            // Check potential moves
            foreach (Vector2 pos in potentialMoves)
            {
                x = (int) pos.X;
                y = (int) pos.Y;
                if ((x < 8) && (x >=0) && (y < 8) && (y >= 0))
                {
                    // Potential move is inside board
                    if (boardSquares[x, y] == ChessBoard.BoardSquare.ENEMY)
                    {
                        boardSquares[x, y] = ChessBoard.BoardSquare.CAN_TAKE;
                    }
                    else
                    {
                        boardSquares[x,y] = ChessBoard.BoardSquare.CAN_MOVE;
                    }
                }
            }

        }

        public void Detect(DetectionResults detectionResults)
        {
            // Initialize results to null
            mBoard.setDetectionResult(null);
            mSelector.setDetectionResult(null);
            mSelector.setBoardMarker(null);

            // Iterate through results
            foreach (DetectionResult result in detectionResults)
            {
                switch (result.Marker.Name)
                {
                    case "selection_marker":
                        mSelector.setDetectionResult(result);
                        break;
                    case "board_marker":
                        mBoard.setDetectionResult(result);
                        mSelector.setBoardMarker(result);
                        break;
                    default:
                        break;
                }
            }

            // Manage Selection
            //setSelected( mSelector.getSelected() );
        }

        public void Draw()
        {
            // Draw Board
            mBoard.Draw();
            DetectionResult boardMarker = mBoard.getDetectionResult();

            foreach (ChessPiece chessPiece in mChessPieces)
            {
                if (chessPiece == mSelectedPiece)
                {
                    DetectionResult selectorMarker = mSelector.getDetectionResult();
                    chessPiece.Draw(selectorMarker);
                }
                else
                {
                    chessPiece.Draw( boardMarker );
                }
            }

            // Draw Selector
            mSelector.Draw();
        }

        public CurrentGameState toCurrentGameState()
        {
            return null;
        }
    }
}
