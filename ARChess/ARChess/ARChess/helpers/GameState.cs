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
        private string selectedPieceMasqueradingAs;
        private ContentManager content;

        private ChessBoard mBoard;
        private PieceSelector mSelector;
        private bool mMoveMade = false;
        private CurrentGameState mCurrentState;

        private static GameState mInstance = null;
        private Vector2 previousPosition;
        private Vector2 moveAlongPosition;
        private Vector2 chosenPosition = Vector2.Zero;
        private Vector2 velocity = Vector2.Zero;

        public static GameState getInstance(bool createIfNotExistent = true)
        {
            if (mInstance == null && createIfNotExistent)
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
            System.Diagnostics.Debug.WriteLine("Initializing");
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
            if (state != null)
            {
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

                chessPieces["black_pawn1"].setMasqueradesAs(state.black.pawn1.masquerading_as);
                chessPieces["black_pawn2"].setMasqueradesAs(state.black.pawn2.masquerading_as);
                chessPieces["black_pawn3"].setMasqueradesAs(state.black.pawn3.masquerading_as);
                chessPieces["black_pawn4"].setMasqueradesAs(state.black.pawn4.masquerading_as);
                chessPieces["black_pawn5"].setMasqueradesAs(state.black.pawn5.masquerading_as);
                chessPieces["black_pawn6"].setMasqueradesAs(state.black.pawn6.masquerading_as);
                chessPieces["black_pawn7"].setMasqueradesAs(state.black.pawn7.masquerading_as);
                chessPieces["black_pawn8"].setMasqueradesAs(state.black.pawn8.masquerading_as);
                chessPieces["black_rook1"].setMasqueradesAs(state.black.rook1.masquerading_as);
                chessPieces["black_rook2"].setMasqueradesAs(state.black.rook2.masquerading_as);
                chessPieces["black_bishop1"].setMasqueradesAs(state.black.bishop1.masquerading_as);
                chessPieces["black_bishop2"].setMasqueradesAs(state.black.bishop2.masquerading_as);
                chessPieces["black_knight1"].setMasqueradesAs(state.black.knight1.masquerading_as);
                chessPieces["black_knight2"].setMasqueradesAs(state.black.knight2.masquerading_as);
                chessPieces["black_queen"].setMasqueradesAs(state.black.queen.masquerading_as);
                chessPieces["black_king"].setMasqueradesAs(state.black.king.masquerading_as);

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

                chessPieces["white_pawn1"].setMasqueradesAs(state.white.pawn1.masquerading_as);
                chessPieces["white_pawn2"].setMasqueradesAs(state.white.pawn2.masquerading_as);
                chessPieces["white_pawn3"].setMasqueradesAs(state.white.pawn3.masquerading_as);
                chessPieces["white_pawn4"].setMasqueradesAs(state.white.pawn4.masquerading_as);
                chessPieces["white_pawn5"].setMasqueradesAs(state.white.pawn5.masquerading_as);
                chessPieces["white_pawn6"].setMasqueradesAs(state.white.pawn6.masquerading_as);
                chessPieces["white_pawn7"].setMasqueradesAs(state.white.pawn7.masquerading_as);
                chessPieces["white_pawn8"].setMasqueradesAs(state.white.pawn8.masquerading_as);
                chessPieces["white_rook1"].setMasqueradesAs(state.white.rook1.masquerading_as);
                chessPieces["white_rook2"].setMasqueradesAs(state.white.rook2.masquerading_as);
                chessPieces["white_bishop1"].setMasqueradesAs(state.white.bishop1.masquerading_as);
                chessPieces["white_bishop2"].setMasqueradesAs(state.white.bishop2.masquerading_as);
                chessPieces["white_knight1"].setMasqueradesAs(state.white.knight1.masquerading_as);
                chessPieces["white_knight2"].setMasqueradesAs(state.white.knight2.masquerading_as);
                chessPieces["white_queen"].setMasqueradesAs(state.white.queen.masquerading_as);
                chessPieces["white_king"].setMasqueradesAs(state.white.king.masquerading_as);

                mMyColor = GameStateManager.getInstance().getCurrentPlayer();
                setPieceMoves();
            }
        }

        private void initializePlayerPieces(ChessPiece.Color _player)
        {
            Vector2 position;
            // Initialize Pawns
            for (int i = 0; i < 8; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 6 : 1), i);
                chessPieces[(_player == ChessPiece.Color.BLACK ? "black" : "white") + "_pawn" + (i + 1).ToString()] = new ChessPiece(content, _player, ChessPiece.Piece.PAWN, position, "pawn");
            }

            // Initialize Knights
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (i == 0 ? 1 : 6));
                chessPieces[(_player == ChessPiece.Color.BLACK ? "black" : "white") + "_knight" + (i + 1).ToString()] = new ChessPiece(content, _player, ChessPiece.Piece.KNIGHT, position, "knight");
            }

            // Initialize Bishops
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (i == 0 ? 2 : 5));
                chessPieces[(_player == ChessPiece.Color.BLACK ? "black" : "white") + "_bishop" + (i + 1).ToString()] = new ChessPiece(content, _player, ChessPiece.Piece.BISHOP, position, "bishop");
            }

            // Initialize Rooks
            for (int i = 0; i < 2; i++)
            {
                position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (i == 0 ? 0 : 7));
                chessPieces[(_player == ChessPiece.Color.BLACK ? "black" : "white") + "_rook" + (i + 1).ToString()] = new ChessPiece(content, _player, ChessPiece.Piece.ROOK, position, "rook");
            }

            // Initialize Queens
            position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (_player == ChessPiece.Color.BLACK ? 3 : 4));
            chessPieces[(_player == ChessPiece.Color.BLACK ? "black" : "white") + "_queen"] = new ChessPiece(content, _player, ChessPiece.Piece.QUEEN, position, "queen");

            // Initialize Kings
            position = new Vector2((_player == ChessPiece.Color.BLACK ? 7 : 0), (_player == ChessPiece.Color.BLACK ? 4 : 3));
            chessPieces[(_player == ChessPiece.Color.BLACK ? "black" : "white") + "_king"] = new ChessPiece(content, _player, ChessPiece.Piece.KING, position, "king");
        }

        private void setPieceMoves()
        {
            foreach (KeyValuePair<string, ChessPiece> entry in chessPieces)
            {
                if (!entry.Value.isTaken())
                {
                    //System.Diagnostics.Debug.WriteLine("Setting Moves for: >" + entry.Value.getMasqueradeType() + "<");
                    entry.Value.determineMoves(chessPieces);
                }
            }
        }

        private List<ChessPiece> checkingPieces = new List<ChessPiece>();
        private List<ChessPiece> guardingPieces = new List<ChessPiece>();
        public bool inCheck(ChessPiece.Color playerColor)
        {
            setPieceMoves();
            checkingPieces.Clear();
            guardingPieces.Clear();
            bool inCheck = false;

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
                        checkingPieces.Add(entry.Value);
                        inCheck = true;
                    }
                }
            }
            return inCheck;
        }

        public bool checkmate(ChessPiece.Color opponentColor)
        {
            // Assume inCheck() has been called,
            // therefore all piece moves have been calculated

            //Cache current state
            mCurrentState = toCurrentGameState();
            ChessPiece kingPiece = chessPieces[(opponentColor == ChessPiece.Color.BLACK ? "black" : "white") + "_king"];
            Vector2 kingPos = kingPiece.getPosition();

            // If king has a move that is not CAN_TAKE or CAN_MOVE
            // Check if any of kings moves do not result in check
            int[] cardinalDir = new int[16]{-1,1,  0,1,  1,1,
                                            -1,0,        1,0,
                                            -1,-1, 0,-1, 1,-1 };

            for (int i = 0; i < 8; ++i)
            {
                //System.Diagnostics.Debug.WriteLine("Check # " + i);

                Vector2 potentialMove = kingPos + new Vector2(cardinalDir[2 * i], cardinalDir[2 * i + 1]);
                //System.Diagnostics.Debug.WriteLine(potentialMove);
                try
                {

                    //setSelected(potentialMove);
                    kingPiece.makeMove(potentialMove, chessPieces);
                    if (!inCheck(opponentColor))
                    {
                        // Valid move out of check exists
                        //System.Diagnostics.Debug.WriteLine("CheckMate Check - Valid Move :)");
                        resetTurn();
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    // Not valid move
                    //System.Diagnostics.Debug.WriteLine("CheckMate Check - Exception");
                }
                // Reset and try new move
                resetTurn();
                //System.Diagnostics.Debug.WriteLine("CheckMate Check - Invalid Move");
                //System.Diagnostics.Debug.WriteLine("");
            }
            // All moves have been checked, king is stuck.

            inCheck(opponentColor);
            if (checkingPieces.Count > 1) {
                // If checked by two pieces & can't move, checkmate
                return true;
            }

            // But can he be saved without being exposed to new attack?

            ChessPiece checkingPiece = checkingPieces[0];
            int x = (int)checkingPiece.getPosition().X,
                y = (int)checkingPiece.getPosition().Y,
                xDelta = x - (int)kingPos.X,
                yDelta = y - (int)kingPos.Y,
                dist = (xDelta == 0 ? Math.Abs(yDelta) : Math.Abs(xDelta));
            xDelta /= Math.Abs(xDelta);
            yDelta /= Math.Abs(yDelta);

            foreach (KeyValuePair<string, ChessPiece> entry in chessPieces)
            {
                // Only check pieces in play AND opponents color AND not the king
                if ( (!entry.Value.isTaken()) && 
                     (entry.Value.getPlayer() == opponentColor) &&
                     (entry.Value.getType() == ChessPiece.Piece.KING) )
                {
                    ChessBoard.BoardSquare[,] moves = entry.Value.getMoves();
                    // Check if piece can be taken or blocked if sliding

                    if (moves[x, y] == ChessBoard.BoardSquare.CAN_TAKE)
                    {
                        // Checking piece can be taken
                        entry.Value.makeMove(new Vector2(x, y), chessPieces);
                        if (!inCheck(opponentColor))
                        {
                            // Move does not result in another check
                            //System.Diagnostics.Debug.WriteLine("CheckMate Check - Potential Taking Piece");
                            //System.Diagnostics.Debug.WriteLine("CheckMate Check - " + entry.Value.getPosition());

                            resetTurn();
                            return false;
                        }
                        else
                        {
                            // Move results in check
                            resetTurn();
                        }

                    }

                    if (checkingPieces[0].getType() != ChessPiece.Piece.PAWN &&
                        checkingPieces[0].getType() != ChessPiece.Piece.KNIGHT)
                    {
                        // Check spots between king and checking piece

                        for (int i = 1; i < dist; ++i)
                        {
                            int blockX = (int)kingPos.X + i * xDelta,
                                blockY = (int)kingPos.Y + i * yDelta;

                            if (moves[blockX, blockY] == ChessBoard.BoardSquare.CAN_MOVE)
                            {
                                // Checking piece can be blocked
                                entry.Value.makeMove(new Vector2(blockX, blockY), chessPieces);
                                if (!inCheck(opponentColor))
                                {
                                    // Move does not result in another check
                                    //System.Diagnostics.Debug.WriteLine("CheckMate Check - Potential Blocking Piece");
                                    //System.Diagnostics.Debug.WriteLine("CheckMate Check - " + entry.Value.getPosition());

                                    resetTurn();
                                    return false;
                                }
                                else
                                {
                                    // Move results in check
                                    resetTurn();
                                }
                            }
                            // Else cannot block checking piece
                        }
                    }
                    // Else piece is not a blockable piece
                }
            }

            // Otherwise, in checkmate
            return true;
        }

        public void resetTurn()
        {
            if (mSelectedPiece != null)
            {
                mMoveMade = false;
                mSelectedPiece.setPosition(previousPosition);
                mSelectedPiece.setMasqueradesAs(selectedPieceMasqueradingAs);
                mSelectedPiece = null;
            }
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
                        selectedPieceMasqueradingAs = entry.Value.getMasqueradeType();

                        // In easy mode - set moves to display
                        AppSettings settings = new AppSettings();
                        if (!settings.AdvancedModeSettings)
                        {
                            mBoard.setMoves( entry.Value.getMoves() );
                        }

                        return;
                    }
                }
            }
            else
            {
                // Piece already selected
                ChessBoard.BoardSquare[,] squares = mSelectedPiece.getMoves();
                
                if (mSelectedPiece.getPosition() == newPosition) 
                {
                    // Put Piece Back
                    mSelectedPiece = null;
                    //mBoard.clearBoardSquares();
                }
                else if ( squares[x, y] == ChessBoard.BoardSquare.CAN_MOVE )
                {
                    // Move is valid
                    previousPosition = mSelectedPiece.getPosition();
                    chosenPosition = newPosition;
                    //mBoard.clearBoardSquares();
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

                    previousPosition = mSelectedPiece.getPosition();
                    chosenPosition = newPosition;
                    //mBoard.clearBoardSquares();
                    mMoveMade = true;
                }
                else
                {
                    resetTurn();
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

        public Vector2 getChosenPosition()
        {
            return chosenPosition;
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
                if (entry.Value == mSelectedPiece && !mMoveMade)
                {
                    DetectionResult selectorMarker = mSelector.getDetectionResult();
                    entry.Value.Draw(selectorMarker, new Vector3(4,4,0));
                }
                else if (entry.Value == mSelectedPiece && mSelectedPiece.getPosition() != chosenPosition)
                {
                    if (velocity == Vector2.Zero)
                    {
                        Vector2 startPosition = previousPosition;
                        startPosition -= new Vector2(4, 4);
                        startPosition *= ModelDrawer.SCALE;
                        Vector2 endPosition = chosenPosition;
                        endPosition -= new Vector2(4, 4);
                        endPosition *= ModelDrawer.SCALE;

                        velocity = endPosition - startPosition;
                        velocity.Normalize();
                        velocity = velocity * 1.3f;

                        moveAlongPosition = startPosition;
                    }

                    moveAlongPosition += velocity;
                    entry.Value.DrawAtLocation(boardMarker, moveAlongPosition);

                    if ((int)(Vector2.Distance(moveAlongPosition, (chosenPosition - new Vector2(4,4)) * ModelDrawer.SCALE)) < ModelDrawer.SCALE / 10)
                    {
                        entry.Value.setPosition(chosenPosition);
                        velocity = Vector2.Zero;
                    }
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

        public CurrentGameState toCurrentGameState()
        {
            return new CurrentGameState()
            {
                black = new PlayerState()
                {
                    in_check = inCheck(ChessPiece.Color.BLACK),
                    pawn1 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn1"].getPosition().X,
                        y = (int)chessPieces["black_pawn1"].getPosition().Y,
                        masquerading_as = chessPieces["black_pawn1"].getMasqueradeType()
                    },
                    pawn2 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn2"].getPosition().X,
                        y = (int)chessPieces["black_pawn2"].getPosition().Y,
                        masquerading_as = chessPieces["black_pawn2"].getMasqueradeType()
                    },
                    pawn3 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn3"].getPosition().X,
                        y = (int)chessPieces["black_pawn3"].getPosition().Y,
                        masquerading_as = chessPieces["black_pawn3"].getMasqueradeType()
                    },
                    pawn4 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn4"].getPosition().X,
                        y = (int)chessPieces["black_pawn4"].getPosition().Y,
                        masquerading_as = chessPieces["black_pawn4"].getMasqueradeType()
                    },
                    pawn5 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn5"].getPosition().X,
                        y = (int)chessPieces["black_pawn5"].getPosition().Y,
                        masquerading_as = chessPieces["black_pawn5"].getMasqueradeType()
                    },
                    pawn6 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn6"].getPosition().X,
                        y = (int)chessPieces["black_pawn6"].getPosition().Y,
                        masquerading_as = chessPieces["black_pawn6"].getMasqueradeType()
                    },
                    pawn7 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn7"].getPosition().X,
                        y = (int)chessPieces["black_pawn7"].getPosition().Y,
                        masquerading_as = chessPieces["black_pawn7"].getMasqueradeType()
                    },
                    pawn8 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_pawn8"].getPosition().X,
                        y = (int)chessPieces["black_pawn8"].getPosition().Y,
                        masquerading_as = chessPieces["black_pawn8"].getMasqueradeType()
                    },
                    rook1 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_rook1"].getPosition().X,
                        y = (int)chessPieces["black_rook1"].getPosition().Y,
                        masquerading_as = chessPieces["black_rook1"].getMasqueradeType()
                    },
                    rook2 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_rook2"].getPosition().X,
                        y = (int)chessPieces["black_rook2"].getPosition().Y,
                        masquerading_as = chessPieces["black_rook2"].getMasqueradeType()
                    },
                    bishop1 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_bishop1"].getPosition().X,
                        y = (int)chessPieces["black_bishop1"].getPosition().Y,
                        masquerading_as = chessPieces["black_bishop1"].getMasqueradeType()
                    },
                    bishop2 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_bishop2"].getPosition().X,
                        y = (int)chessPieces["black_bishop2"].getPosition().Y,
                        masquerading_as = chessPieces["black_bishop2"].getMasqueradeType()
                    },
                    knight1 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_knight1"].getPosition().X,
                        y = (int)chessPieces["black_knight1"].getPosition().Y,
                        masquerading_as = chessPieces["black_knight1"].getMasqueradeType()
                    },
                    knight2 = new PieceLocation()
                    {
                        x = (int)chessPieces["black_knight2"].getPosition().X,
                        y = (int)chessPieces["black_knight2"].getPosition().Y,
                        masquerading_as = chessPieces["black_knight2"].getMasqueradeType()
                    },
                    queen = new PieceLocation()
                    {
                        x = (int)chessPieces["black_queen"].getPosition().X,
                        y = (int)chessPieces["black_queen"].getPosition().Y,
                        masquerading_as = chessPieces["black_queen"].getMasqueradeType()
                    },
                    king = new PieceLocation()
                    {
                        x = (int)chessPieces["black_king"].getPosition().X,
                        y = (int)chessPieces["black_king"].getPosition().Y,
                        masquerading_as = chessPieces["black_king"].getMasqueradeType()
                    }
                },
                white = new PlayerState()
                {
                    in_check = inCheck(ChessPiece.Color.WHITE),
                    pawn1 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn1"].getPosition().X,
                        y = (int)chessPieces["white_pawn1"].getPosition().Y,
                        masquerading_as = chessPieces["white_pawn1"].getMasqueradeType()
                    },
                    pawn2 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn2"].getPosition().X,
                        y = (int)chessPieces["white_pawn2"].getPosition().Y,
                        masquerading_as = chessPieces["white_pawn2"].getMasqueradeType()
                    },
                    pawn3 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn3"].getPosition().X,
                        y = (int)chessPieces["white_pawn3"].getPosition().Y,
                        masquerading_as = chessPieces["white_pawn3"].getMasqueradeType()
                    },
                    pawn4 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn4"].getPosition().X,
                        y = (int)chessPieces["white_pawn4"].getPosition().Y,
                        masquerading_as = chessPieces["white_pawn4"].getMasqueradeType()
                    },
                    pawn5 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn5"].getPosition().X,
                        y = (int)chessPieces["white_pawn5"].getPosition().Y,
                        masquerading_as = chessPieces["white_pawn5"].getMasqueradeType()
                    },
                    pawn6 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn6"].getPosition().X,
                        y = (int)chessPieces["white_pawn6"].getPosition().Y,
                        masquerading_as = chessPieces["white_pawn6"].getMasqueradeType()
                    },
                    pawn7 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn7"].getPosition().X,
                        y = (int)chessPieces["white_pawn7"].getPosition().Y,
                        masquerading_as = chessPieces["white_pawn7"].getMasqueradeType()
                    },
                    pawn8 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_pawn8"].getPosition().X,
                        y = (int)chessPieces["white_pawn8"].getPosition().Y,
                        masquerading_as = chessPieces["white_pawn8"].getMasqueradeType()
                    },
                    rook1 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_rook1"].getPosition().X,
                        y = (int)chessPieces["white_rook1"].getPosition().Y,
                        masquerading_as = chessPieces["white_rook1"].getMasqueradeType()
                    },
                    rook2 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_rook2"].getPosition().X,
                        y = (int)chessPieces["white_rook2"].getPosition().Y,
                        masquerading_as = chessPieces["white_rook2"].getMasqueradeType()
                    },
                    bishop1 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_bishop1"].getPosition().X,
                        y = (int)chessPieces["white_bishop1"].getPosition().Y,
                        masquerading_as = chessPieces["white_bishop1"].getMasqueradeType()
                    },
                    bishop2 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_bishop2"].getPosition().X,
                        y = (int)chessPieces["white_bishop2"].getPosition().Y,
                        masquerading_as = chessPieces["white_bishop2"].getMasqueradeType()
                    },
                    knight1 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_knight1"].getPosition().X,
                        y = (int)chessPieces["white_knight1"].getPosition().Y,
                        masquerading_as = chessPieces["white_knight1"].getMasqueradeType()
                    },
                    knight2 = new PieceLocation()
                    {
                        x = (int)chessPieces["white_knight2"].getPosition().X,
                        y = (int)chessPieces["white_knight2"].getPosition().Y,
                        masquerading_as = chessPieces["white_knight2"].getMasqueradeType()
                    },
                    queen = new PieceLocation()
                    {
                        x = (int)chessPieces["white_queen"].getPosition().X,
                        y = (int)chessPieces["white_queen"].getPosition().Y,
                        masquerading_as = chessPieces["white_queen"].getMasqueradeType()
                    },
                    king = new PieceLocation()
                    {
                        x = (int)chessPieces["white_king"].getPosition().X,
                        y = (int)chessPieces["white_king"].getPosition().Y,
                        masquerading_as = chessPieces["white_king"].getMasqueradeType()
                    }
                }
            };
        }
    }
}
