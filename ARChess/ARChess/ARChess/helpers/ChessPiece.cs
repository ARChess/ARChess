using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using SLARToolKit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ARChess
{
    public class ChessPiece
    {
        public enum Color { WHITE, BLACK };

        public enum Piece { PAWN, ROOK, KNIGHT, BISHOP, QUEEN, KING };

        private Piece mType;
        private Color mPlayer;
        private Model mModel;
        private Vector2 mPosition;
        private ContentManager content;

        private List<ChessPiece> mPieces;

        public ChessPiece(ContentManager _content, XElement pieceElement)
        {
            // Player
            XElement playerNode = pieceElement.Element("Player");
            String playerString = playerNode.Value;
            Color player;
            if (playerString == "WHITE")
            {
                player = Color.WHITE;
            }
            else
            {
                player = Color.BLACK;
            }

            // Type
            XElement typeNode = pieceElement.Element("Type");
            String typeString = typeNode.Value;
            Piece type = Piece.PAWN;
            switch (typeString)
            {
                case "PAWN" :
                    type = Piece.PAWN;
                    break;
                case "ROOK" :
                    type = Piece.ROOK;
                    break;
                case "KNIGHT" :
                    type = Piece.KNIGHT;
                    break;
                case "BISHOP" :
                    type = Piece.BISHOP;
                    break;
                case "QUEEN" :
                    type = Piece.QUEEN;
                    break;
                case "KING" :
                    type = Piece.KING;
                    break;
            }

            // Position
            XElement positionNode = pieceElement.Element("Position");
            String positionXString = positionNode.Attribute("X").Value;
            String positionYString = positionNode.Attribute("Y").Value;

            int x = System.Convert.ToInt32(positionXString);
            int y = System.Convert.ToInt32(positionYString);

            Vector2 position = new Vector2(x, y);

            initialize(_content, player, type, position);
        }

        public ChessPiece(ContentManager _content, Color _player, Piece _type, Vector2 _position)
        {
            initialize(_content, _player, _type, _position);
        }

        private void initialize(ContentManager _content, Color _player, Piece _type, Vector2 _position)
        {
            mType = _type;
            mPlayer = _player;
            content = _content;


            switch (_type)
            {
                case Piece.PAWN:
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_PAWN : ModelSelector.Pieces.BLACK_PAWN);
                    break;
                case Piece.ROOK:
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_ROOK : ModelSelector.Pieces.BLACK_ROOK);
                    break;
                case Piece.KNIGHT:
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_KNIGHT : ModelSelector.Pieces.BLACK_KNIGHT);
                    break;
                case Piece.BISHOP:
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_BISHOP : ModelSelector.Pieces.BLACK_BISHOP);
                    break;
                case Piece.QUEEN:
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_QUEEN : ModelSelector.Pieces.BLACK_QUEEN);
                    break;
                case Piece.KING:
                    mModel = ModelSelector.getModel(_player == Color.WHITE ? ModelSelector.Pieces.WHITE_KING : ModelSelector.Pieces.BLACK_KING);
                    break;
            }

            mPosition = _position;
        }

        public Piece getType()
        {
            return mType;
        }

        public Color getPlayer()
        {
            return mPlayer;
        }

        public Model getModel()
        {
            return mModel;
        }

        public Vector2 getPosition()
        {
            return mPosition;
        }

        public void setPosition(Vector2 _position)
        {
            mPosition = _position;

            // Constrain position to board
            if (mPosition.X < 0) { mPosition.X = 0; }
            else if (mPosition.X > 7) { mPosition.X = 7; }
            if (mPosition.Y < 0) { mPosition.Y = 0; }
            else if (mPosition.Y > 7) { mPosition.Y = 7; }
        }

        public void setPieceList(List<ChessPiece> pieces)
        {
            mPieces = pieces;
        }

        public ChessBoard.BoardSquare[,] getBoardSquares()
        {
            // Set entire board to OPEN
            ChessBoard.BoardSquare[,] boardSquares = new ChessBoard.BoardSquare[8,8];
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    boardSquares[i,j] = ChessBoard.BoardSquare.OPEN;
                }
            }
            // Load FRIEND and ENEMY positions onto squares
            foreach (ChessPiece piece in mPieces)
            {
                Vector2 pos = piece.getPosition();
                if (piece.getPlayer() == mPlayer)
                {
                    boardSquares[(int) pos.X, (int) pos.Y] = ChessBoard.BoardSquare.FRIEND;
                }
                else
                {
                    boardSquares[(int)pos.X, (int)pos.Y] = ChessBoard.BoardSquare.ENEMY;
                }
            }

            // Based on Type, determine potential moves
            int forward = (mPlayer == Color.WHITE ? 1 : -1);
            int x = (int) mPosition.X;
            int y = (int) mPosition.Y;
            List<Vector2> potentialMoves = new List<Vector2>();
            List<Vector2> slideDirection = new List<Vector2>();

            switch (mType)
            {
                case Piece.PAWN :
                    potentialMoves.Add( new Vector2(x + forward, y - 1) );
                    potentialMoves.Add( new Vector2(x + forward, y) );
                    potentialMoves.Add( new Vector2(x + forward, y + 1) );
                    break;

                case Piece.ROOK:
                    slideDirection.Add( new Vector2( 0 ,  1) );
                    slideDirection.Add( new Vector2( 0 , -1) );
                    slideDirection.Add( new Vector2( 1 ,  0) );
                    slideDirection.Add( new Vector2(-1 ,  0) );

                    if (mType != Piece.ROOK)
                    {
                        // QUEEN is a Rook and Bishop combined
                        goto case Piece.BISHOP;
                    }
                    break;

                case Piece.KNIGHT:
                    // TODO
                    break;

                case Piece.BISHOP:
                    slideDirection.Add( new Vector2(1,1) );
                    slideDirection.Add( new Vector2(-1,1) );
                    slideDirection.Add( new Vector2(1,-1) );
                    slideDirection.Add( new Vector2(-1,-1) );  
                    break;

                case Piece.QUEEN:
                    // Essentially a Rook and Bishop combined
                    goto case Piece.ROOK;

                case Piece.KING:
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
                //int xEdge = xDelta != 0 ? (7 - xDelta * x) % 7 : 7;
                //int yEdge = yDelta != 0 ? (7 - yDelta * y) % 7 : 7;
                //int closestEdge = (xEdge < yEdge ? xEdge : yEdge);
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

            return boardSquares;

        }

        public void Draw(DetectionResult markerResult)
        {
            if(getType() == ChessPiece.Piece.KNIGHT)
            {
                if(getPlayer() == ChessPiece.Color.BLACK)
                {
                    ModelDrawer.Draw(markerResult, mModel, (int)mPosition.X, (int)mPosition.Y, .2, -1.5707f); //rotated by -90 degress
                }
                else
                {
                    ModelDrawer.Draw(markerResult, mModel, (int)mPosition.X, (int)mPosition.Y, .2, 1.5707f); //rotated by 90 degress
                }
            }
            else
            {
                ModelDrawer.Draw(markerResult, mModel, (int)mPosition.X, (int)mPosition.Y, .2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XElement toXml()
        {
            XElement pieceElement = new XElement("ChessPiece");

            // Player
            pieceElement.Add(new XElement("Player", mPlayer.ToString()));

            // Type
            pieceElement.Add(new XElement("Type", mType.ToString()));

            // Position
            pieceElement.Add(new XElement("Position", new XAttribute("X", mPosition.X), new XAttribute("Y", mPosition.Y)));
            
            return pieceElement;
        }

    }
}
