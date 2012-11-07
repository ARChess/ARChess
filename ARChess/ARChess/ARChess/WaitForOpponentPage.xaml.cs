using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ARChess
{
    public partial class WaitForOpponentPage : PhoneApplicationPage
    {
        private GameResponse response;

        public WaitForOpponentPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (s, args) =>
            {
                response = new NetworkTask().getGameState();
                while (response.is_game_over == false && response.is_current_players_turn == false)
                {
                    Thread.Sleep(10000);
                    response = new NetworkTask().getGameState();
                }
            };
            bw.RunWorkerCompleted += (s, args) =>
            {
                if (response.is_game_over == false)
                {
                    NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
                }
                else
                {
                    if (response.winner == response.current_player)
                    {
                        NavigationService.Navigate(new Uri("/WonPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        NavigationService.Navigate(new Uri("/LostPage.xaml", UriKind.Relative));
                    }
                }
            };
            bw.RunWorkerAsync();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}