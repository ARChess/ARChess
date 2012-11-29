using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ARChess
{
    public class GameStateManager
    {
        private static GameStateManager instance = null;
        private static ChessPiece.Color currentPlayer = ChessPiece.Color.WHITE;
        private static CurrentGameState currentState = null;

        public static GameStateManager getInstance()
        {
            if (instance == null)
            {
                instance = new GameStateManager();
            }
            return instance;
        }

        public void setGameState(CurrentGameState _instance)
        {
            currentState = _instance;
        }

        public CurrentGameState getGameState()
        {
            return currentState;
        }

        public void setCurrentPlayer(string player)
        {
            if (player == "black")
            {
                currentPlayer = ChessPiece.Color.BLACK;
            }
            else
            {
                currentPlayer = ChessPiece.Color.WHITE;
            }
        }

        public ChessPiece.Color getCurrentPlayer()
        {
            return currentPlayer;
        }
    }

    public class PieceLocation
    {
        public int x { get; set; }
        public int y { get; set; }
        public string masquerading_as { get; set; }
    }

    public class PlayerState
    {
        public boolean in_check { get; set; }
        public PieceLocation pawn1 { get; set; }
        public PieceLocation pawn2 { get; set; }
        public PieceLocation pawn3 { get; set; }
        public PieceLocation pawn4 { get; set; }
        public PieceLocation pawn5 { get; set; }
        public PieceLocation pawn6 { get; set; }
        public PieceLocation pawn7 { get; set; }
        public PieceLocation pawn8 { get; set; }
        public PieceLocation rook1 { get; set; }
        public PieceLocation rook2 { get; set; }
        public PieceLocation knight1 { get; set; }
        public PieceLocation knight2 { get; set; }
        public PieceLocation bishop1 { get; set; }
        public PieceLocation bishop2 { get; set; }
        public PieceLocation queen { get; set; }
        public PieceLocation king { get; set; }
    }

    public class CurrentGameState
    {
        public PlayerState black { get; set; }
        public PlayerState white { get; set; }
    }

    public class GameResponse
    {
        public bool game_in_progress { get; set; }
        public string player_color { get; set; }
        public bool is_current_players_turn { get; set; }
        public string current_player { get; set; }
        public bool is_game_over { get; set; }
        public string winner { get; set; }
        public CurrentGameState game_state { get; set; }
    }
}
