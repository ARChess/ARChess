using System;
using System.IO;
using System.Net;
using System.Text;
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
    public class NetworkTask
    {
        public NetworkTask()
        {
            
        }

        private void makeHttpRequest(string path, string verb, string data)
        {

        }

        public GameState getGameState()
        {
            return null;
        }

        public bool sendGameState(GameState state)
        {
            return true;
        }

        public bool resignGame()
        {
            return true;
        }
    }
}
