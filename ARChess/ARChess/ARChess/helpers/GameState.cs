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
        private bool mMoveMade = false;
        private CurrentGameState mCurrentState;

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
            setPieceMoves();
        }

        public void loadState(CurrentGameState state)
        {
            mMoveMade = false;
            mCurrentState = state;

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

            chessPieces["white_pawn1"].setPosition(new Vector2((float)state.white.pawn1.x, (float)state.white.pawn1.y));
            chessPieces["white_pawn2"].setPosition(new Vector2((float)state.white.pawn2.x, (float)state.white.pawn2.y));
            chessPieces["white_pawn2"].setPosition(new Vector2((float)state.white.pawn2.x, (float)state.white.pawn2.y));
            chessPieces["white_pawn3"].setPosition(new Vector2((float)state.white.pawn3.x, (float)state.white.pawn3.y));
            chessPieces["white_pawn4"].setPosition(new Vector2((float)state.white.pawn4.x, (float)state.white.pawn4.y));
            chessPieces["white_pawn5"].setPosition(new Vector2((float)state.white.pawn5.x, (float)state.white.pawn5.y));
            chessPieces["white_pawn6"].setPosition(new Vector2((float)state.white.pawn6.x, (float)state.white.pawn6.y));
            chessPieces["white_pawn7"].setPosition(new Vector2((float)state.white.pawn7.x, (float)state.white.pawn7.y));
            chessPieces["white_pawn8"].setPosition(new Vector2((float)state.white.pawn8.x, (float)state.white.pawn8.y));
            chessPieces["white_rook1"].setPosition(new Vector2((float)state.white.rook1.x, (float)state.white.rook1.y));
            chessPieces["white_rook2"].setPosition(new Vector2((float)state.white.rook2.x, (float)state.white.rook2.y));
            chessPieces["white_bishop1"].setPosition(new Vector2((float)state.white.bishop1.x, (float)state.white.bishop1.y));
            chessPieces["white_bishop2"].setPosition(new Vector2((float)state.white.bishop2.x, (float)state.white.bishop2.y));
            chessPieces["white_knight1"].setPosition(new Vector2((float)state.white.knight1.x, (float)state.white.knight1.y));
            chessPieces["white_knight2"].setPosition(new Vector2((float)state.white.knight2.x, (float)state.white.knight2.y));
            chessPieces["white_queen"].setPosition(new Vector2((float)state.white.queen.x, (float)state.white.queen.y));
            chessPieces["white_king"].setPosition(new Vector2((float)state.white.king.x, (float)state.white.king.y));

            mMyColor = GameStateManager.getInstance().getCurrentPlayer();
            setPieceMoves();
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

        private void setPieceMoves()
        {
            foreach (KeyValuePair<string, ChessPiece> entry in chessPieces)
            {
                if (!entry.Value.isTaken())
                {
                    entry.Value.determineMoves(chessPieces);
                }
            }
        }

        public bool inCheck(ChessPiece.Color playerColor)
        {
            setPieceMoves();

            Vector2 kingPosition = chessPieces[(playerColor == ChessPiece.Color.BLACK ? "black" : "white") + "_king"].getPosition();
            int x = (int)kingPosition.X;
            int y = (int)kingPosition.Y;

            foreach (KeyValuePair<string, ChessPiece> entry in chessPieces)
            {
                if ((!entry.Value.isTaken()) && (entry.Value.getPlayer() != playerColor))
                {
                    ChessBoard.BoardSquare[,] moves = entry.Value.getMoves();
                    if (moves[x,y] == ChessBoard.BoardSquare.CAN_TAKE)
                    {
                        // Player is in check
                        return true;
                    }
                }
            }
            return false;
        }

        public void resetTurn()
        {
            mSelectedPiece = null;
            mBoard.clearBoardSquares();
            loadState(mCurrentState);
        }

        public Dictionary<string, ChessPiece> getPieces()
        {
            return chessPieces;
        }

        public ChessPiece.Color getMyColor() {
            return mMyColor;
        }

        public void setSelected(Vector2 position)
        {
            int x = (int) position.X;
            int y = (int) position.Y;
            Vector2 newPosition = new Vector2(x, y);

            if (mSelectedPiece == null)
            {
                // Select Piece at position
                foreach (KeyValuePair<string, ChessPiece> entry in chessPieces)
                {
                    if ((mMyColor == entry.Value.getPlayer()) && (newPosition == entry.Value.getPosition()))
                    {
                        // Set Selected Piece
                        mSelectedPiece = entry.Value;

                        // Set moves to display
                        mBoard.setMoves( entry.Value.getMoves() );

                        return;
                    }
                }
            }
            else
            {
                // Piece already selected
                ChessBoard.BoardSquare[,] squares = mBoard.getBoardSquares();
                
                if (mSelectedPiece.getPosition() == newPosition) 
                {
                    // Put Piece Back
                    mSelectedPiece = null;
                    mBoard.clearBoardSquares();
                }
                else if ( squares[x, y] == ChessBoard.BoardSquare.CAN_MOVE )
                {
                    // Move is valid
                    mSelectedPiece.setPosition( newPosition );
                    mSelectedPiece = null;
                    mBoard.clearBoardSquares();
                    mMoveMade = true;
                }
                else if (squares[x, y] == ChessBoard.BoardSquare.CAN_TAKE) 
                {
                    // Move is valid - Enemy Piece is taken
                    foreach (KeyValuePair<string, ChessPiece> entry in chessPieces)
                    {
                        if ((mMyColor != entry.Value.getPlayer()) && (newPosition == entry.Value.getPosition()))
                        {
                            // Remove Enemy Piece
                            entry.Value.remove();
                            break;
                        }
                    }

                    mSelectedPiece.setPosition(newPosition);
                    mSelectedPiece = null;
                    mBoard.clearBoardSquares();
                    mMoveMade = true;
                }
                else
                {
                    // Move is not valid
                    System.Diagnostics.Debug.WriteLine("Invalid Move");
                    System.Diagnostics.Debug.WriteLine(new Vector2(x, y));
                    throw new Exception("Move is not Valid");
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
        }

        public void Update()
        {
            
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
                    entry.Value.Draw(selectorMarker, new Vector3(4,4,0));
                }
                else
                {
                    entry.Value.Draw(boardMarker);
                }
            }

            // Draw Selector
            if (!mMoveMade)
            {
                mSelector.Draw();
            }
        }

        public void processVoiceCommand(string command)
        {
            if (command.ToLower().IndexOf("move") != -1 || command.ToLower().IndexOf("select") != -1)
            {
                VoiceCommandFuzzyProcessing.process(command);
            }
            else
            {
                //not a valid command
            }
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
