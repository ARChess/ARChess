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
using System.Windows.Navigation;

namespace ARChess
{
    public class GameState
    {
        private Dictionary<string, ChessPiece> chessPieces;
        private ChessPiece.Color mMyColor;
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

        private void initialize() 
        {
            chessPieces = new Dictionary<string, ChessPiece>();

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
            chessPieces["black_pawn1"].setPosition(new Vector2((float)state.black.pawn1.x, (float)state.black.pawn1.y));
            chessPieces["black_pawn2"].setPosition(new Vector2((float)state.black.pawn2.x, (float)state.black.pawn2.y));
            chessPieces["black_pawn3"].setPosition(new Vector2((float)state.black.pawn3.x, (float)state.black.pawn3.y));
            chessPieces["black_pawn4"].setPosition(new Vector2((float)state.black.pawn4.x, (float)state.black.pawn4.y));
            chessPieces["black_pawn5"].setPosition(new Vector2((float)state.black.pawn5.x, (float)state.black.pawn5.y));
            chessPieces["black_pawn6"].setPosition(new Vector2((float)state.black.pawn6.x, (float)state.black.pawn6.y));
            chessPieces["black_pawn7"].setPosition(new Vector2((float)state.black.pawn7.x, (float)state.black.pawn7.y));
            chessPieces["black_pawn8"].setPosition(new Vector2((float)state.black.pawn8.x, (float)state.black.pawn8.y));
            chessPieces["black_rook1"].setPosition(new Vector2((float)state.black.rook1.x, (float)state.black.rook1.y));
            chessPieces["black_rook2"].setPosition(new Vector2((float)state.black.rook2.x, (float)state.black.rook2.y));
            chessPieces["black_bishop1"].setPosition(new Vector2((float)state.black.bishop1.x, (float)state.black.bishop1.y));
            chessPieces["black_bishop2"].setPosition(new Vector2((float)state.black.bishop2.x, (float)state.black.bishop2.y));
            chessPieces["black_knight1"].setPosition(new Vector2((float)state.black.knight1.x, (float)state.black.knight1.y));
            chessPieces["black_knight2"].setPosition(new Vector2((float)state.black.knight2.x, (float)state.black.knight2.y));
            chessPieces["black_queen"].setPosition(new Vector2((float)state.black.queen.x, (float)state.black.queen.y));
            chessPieces["black_king"].setPosition(new Vector2((float)state.black.king.x, (float)state.black.king.y));

            chessPieces["white_pawn1"].setPosition(new Vector2((float)state.white.pawn1.x + 1, (float)state.white.pawn1.y));
            chessPieces["white_pawn2"].setPosition(new Vector2((float)state.white.pawn2.x + 1, (float)state.white.pawn2.y));
            chessPieces["white_pawn2"].setPosition(new Vector2((float)state.white.pawn2.x + 1, (float)state.white.pawn2.y));
            chessPieces["white_pawn3"].setPosition(new Vector2((float)state.white.pawn3.x + 1, (float)state.white.pawn3.y));
            chessPieces["white_pawn4"].setPosition(new Vector2((float)state.white.pawn4.x + 1, (float)state.white.pawn4.y));
            chessPieces["white_pawn5"].setPosition(new Vector2((float)state.white.pawn5.x + 1, (float)state.white.pawn5.y));
            chessPieces["white_pawn6"].setPosition(new Vector2((float)state.white.pawn6.x + 1, (float)state.white.pawn6.y));
            chessPieces["white_pawn7"].setPosition(new Vector2((float)state.white.pawn7.x + 1, (float)state.white.pawn7.y));
            chessPieces["white_pawn8"].setPosition(new Vector2((float)state.white.pawn8.x + 1, (float)state.white.pawn8.y));
            chessPieces["white_rook1"].setPosition(new Vector2((float)state.white.rook1.x, (float)state.white.rook1.y));
            chessPieces["white_rook2"].setPosition(new Vector2((float)state.white.rook2.x, (float)state.white.rook2.y));
            chessPieces["white_bishop1"].setPosition(new Vector2((float)state.white.bishop1.x, (float)state.white.bishop1.y));
            chessPieces["white_bishop2"].setPosition(new Vector2((float)state.white.bishop2.x, (float)state.white.bishop2.y));
            chessPieces["white_knight1"].setPosition(new Vector2((float)state.white.knight1.x, (float)state.white.knight1.y));
            chessPieces["white_knight2"].setPosition(new Vector2((float)state.white.knight2.x, (float)state.white.knight2.y));
            chessPieces["white_queen"].setPosition(new Vector2((float)state.white.queen.x, (float)state.white.queen.y));
            chessPieces["white_king"].setPosition(new Vector2((float)state.white.king.x, (float)state.white.king.y));
        }

        private void initializePlayerPieces(ChessPiece.Color _player)
        {
            Vector2 position;
            // Initialize Pawns
            for (int i = 0; i < 8; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 6 : 2), i);
                chessPieces[(_player == ChessPiece.Color.BLACK ? "black" : "white") + "_pawn" + (i + 1).ToString()] = new ChessPiece(content, _player, ChessPiece.Piece.PAWN, position);
            }

            // Initialize Knights
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (i == 0 ? 1 : 6));
                chessPieces[(_player == ChessPiece.Color.BLACK ? "black" : "white") + "_knight" + (i + 1).ToString()] = new ChessPiece(content, _player, ChessPiece.Piece.KNIGHT, position);
            }

            // Initialize Bishops
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (i == 0 ? 2 : 5));
                chessPieces[(_player == ChessPiece.Color.BLACK ? "black" : "white") + "_bishop" + (i + 1).ToString()] = new ChessPiece(content, _player, ChessPiece.Piece.BISHOP, position);
            }

            // Initialize Rooks
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (i == 0 ? 0 : 7));
                chessPieces[(_player == ChessPiece.Color.BLACK ? "black" : "white") + "_rook" + (i + 1).ToString()] = new ChessPiece(content, _player, ChessPiece.Piece.ROOK, position);
            }

            // Initialize Queens
            position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (_player == ChessPiece.Color.BLACK ? 3 : 4));
            chessPieces[(_player == ChessPiece.Color.BLACK ? "black" : "white") + "_queen"] = new ChessPiece(content, _player, ChessPiece.Piece.QUEEN, position);

            // Initialize Kings
            position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (_player == ChessPiece.Color.BLACK ? 4 : 3));
            chessPieces[(_player == ChessPiece.Color.BLACK ? "black" : "white") + "_king"] = new ChessPiece(content, _player, ChessPiece.Piece.KING, position);
        }

        public void setSelected(Vector2 position)
        {
            if (mSelectedPiece == null)
            {
                // Select Piece at position
                foreach (KeyValuePair<string, ChessPiece> entry in chessPieces)
                {
                    if (position == entry.Value.getPosition())
                    {
                        // Set Selected Piece
                        mSelectedPiece = entry.Value;

                        // Set board squares
                        setBoardSquares();

                        return;
                    }
                }
            }
            else
            {
                // Piece already selected
                ChessBoard.BoardSquare[,] squares = mBoard.getBoardSquares();
                int x = (int) position.X;
                int y = (int) position.Y;
                if ( squares[x, y] == ChessBoard.BoardSquare.CAN_MOVE )
                {
                    // Move is valid
                    mSelectedPiece.setPosition( new Vector2(x, y) );
                    mSelectedPiece = null;
                    mBoard.clearBoardSquares();
                    System.Diagnostics.Debug.WriteLine("Valid Move");
                    // TODO: Send GameState to server
                    //new NetworkTask().sendGameState( toCurrentGameState() );
                    //NavigationService.Navigate(new Uri("/WaitingForOpponentPage.xaml", UriKind.Relative));
                }
                else
                {
                    // Move is not valid
                    System.Diagnostics.Debug.WriteLine("Invalid Move");
                    System.Diagnostics.Debug.WriteLine(new Vector2(x, y));
                    // Give negative feedback
                }
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
            ChessPiece.Piece selectedType     = mSelectedPiece.getType();
            Vector2          selectedPosition = mSelectedPiece.getPosition();

            // Load FRIEND and ENEMY positions onto squares
            foreach (KeyValuePair<string, ChessPiece> entry in chessPieces)
            {
                Vector2 pos = entry.Value.getPosition();
                if (entry.Value.getPlayer() == mMyColor)
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
                for (int j = 1; j < 8; ++j)
                {
                    int boardX = x + j * xDelta;
                    int boardY = y + j * yDelta;
                    if ((boardX > 7) || (boardX < 0) || (boardY > 7) || (boardY < 0))
                    {
                        break;
                    }

                    if (boardSquares[boardX, boardY] == ChessBoard.BoardSquare.FRIEND)
                    {
                        break;
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

        public void Update()
        {
            if (mSelectedPiece == null)
            {
                //setSelected(new Vector2(0, 0));
            }
            else 
            {
                //setSelected(new Vector2(2, 0));
            }
        }

        public void Draw()
        {
            // Draw Board
            mBoard.Draw();
            DetectionResult boardMarker = mBoard.getDetectionResult();

            foreach (KeyValuePair<string, ChessPiece> entry in chessPieces)
            {
                if (entry.Value == mSelectedPiece)
                {
                    DetectionResult selectorMarker = mSelector.getDetectionResult();
                    entry.Value.Draw(selectorMarker);
                }
                else
                {
                    entry.Value.Draw(boardMarker);
                }
            }

            // Draw Selector
            mSelector.Draw();
        }

        public CurrentGameState toCurrentGameState()
        {
            return new CurrentGameState()
            {
                black = new PlayerState()
                {
                    pawn1 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn1"].getPosition().X,
                        y = (int)chessPieces["black_pawn1"].getPosition().Y
                    },
                    pawn2 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn2"].getPosition().X,
                        y = (int)chessPieces["black_pawn2"].getPosition().Y
                    },
                    pawn3 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn3"].getPosition().X,
                        y = (int)chessPieces["black_pawn3"].getPosition().Y
                    },
                    pawn4 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn4"].getPosition().X,
                        y = (int)chessPieces["black_pawn4"].getPosition().Y
                    },
                    pawn5 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn5"].getPosition().X,
                        y = (int)chessPieces["black_pawn5"].getPosition().Y
                    },
                    pawn6 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn6"].getPosition().X,
                        y = (int)chessPieces["black_pawn6"].getPosition().Y
                    },
                    pawn7 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn7"].getPosition().X,
                        y = (int)chessPieces["black_pawn7"].getPosition().Y
                    },
                    pawn8 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn8"].getPosition().X,
                        y = (int)chessPieces["black_pawn8"].getPosition().Y
                    },
                    rook1 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_rook1"].getPosition().X,
                        y = (int)chessPieces["black_rook1"].getPosition().Y
                    },
                    rook2 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_rook2"].getPosition().X,
                        y = (int)chessPieces["black_rook2"].getPosition().Y
                    },
                    bishop1 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_bishop1"].getPosition().X,
                        y = (int)chessPieces["black_bishop1"].getPosition().Y
                    },
                    bishop2 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_bishop2"].getPosition().X,
                        y = (int)chessPieces["black_bishop2"].getPosition().Y
                    },
                    knight1 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_knight1"].getPosition().X,
                        y = (int)chessPieces["black_knight1"].getPosition().Y
                    },
                    knight2 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_knight2"].getPosition().X,
                        y = (int)chessPieces["black_knight2"].getPosition().Y
                    },
                    queen = new PieceLocation()
                    {
                        x = (int)chessPieces["black_queen"].getPosition().X,
                        y = (int)chessPieces["black_queen"].getPosition().Y
                    },
                    king = new PieceLocation()
                    {
                        x = (int)chessPieces["black_king"].getPosition().X,
                        y = (int)chessPieces["black_king"].getPosition().Y
                    }
                },
                white = new PlayerState()
                {
                    pawn1 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn1"].getPosition().X,
                        y = (int)chessPieces["white_pawn1"].getPosition().Y
                    },
                    pawn2 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn2"].getPosition().X,
                        y = (int)chessPieces["white_pawn2"].getPosition().Y
                    },
                    pawn3 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn3"].getPosition().X,
                        y = (int)chessPieces["white_pawn3"].getPosition().Y
                    },
                    pawn4 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn4"].getPosition().X,
                        y = (int)chessPieces["white_pawn4"].getPosition().Y
                    },
                    pawn5 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn5"].getPosition().X,
                        y = (int)chessPieces["white_pawn5"].getPosition().Y
                    },
                    pawn6 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn6"].getPosition().X,
                        y = (int)chessPieces["white_pawn6"].getPosition().Y
                    },
                    pawn7 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn7"].getPosition().X,
                        y = (int)chessPieces["white_pawn7"].getPosition().Y
                    },
                    pawn8 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn8"].getPosition().X,
                        y = (int)chessPieces["white_pawn8"].getPosition().Y
                    },
                    rook1 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_rook1"].getPosition().X,
                        y = (int)chessPieces["white_rook1"].getPosition().Y
                    },
                    rook2 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_rook2"].getPosition().X,
                        y = (int)chessPieces["white_rook2"].getPosition().Y
                    },
                    bishop1 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_bishop1"].getPosition().X,
                        y = (int)chessPieces["white_bishop1"].getPosition().Y
                    },
                    bishop2 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_bishop2"].getPosition().X,
                        y = (int)chessPieces["white_bishop2"].getPosition().Y
                    },
                    knight1 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_knight1"].getPosition().X,
                        y = (int)chessPieces["white_knight1"].getPosition().Y
                    },
                    knight2 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_knight2"].getPosition().X,
                        y = (int)chessPieces["white_knight2"].getPosition().Y
                    },
                    queen = new PieceLocation()
                    {
                        x = (int)chessPieces["white_queen"].getPosition().X,
                        y = (int)chessPieces["white_queen"].getPosition().Y
                    },
                    king = new PieceLocation()
                    {
                        x = (int)chessPieces["white_king"].getPosition().X,
                        y = (int)chessPieces["white_king"].getPosition().Y
                    }
                }
            };
        }
    }
}
