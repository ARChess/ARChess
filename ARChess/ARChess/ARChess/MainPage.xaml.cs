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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ARChess
{
    public partial class MainPage : PhoneApplicationPage
    {
        private GameResponse response;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (s, args) =>
            {
                response = new NetworkTask().getGameState();
            };
            bw.RunWorkerCompleted += (s, args) =>
            {
                if (response.game_in_progress == true)
                {
                    StartButton.Content = "Continue Game";
                }
                else
                {
                    StartButton.Content = "Start Game";
                }
            };
            bw.RunWorkerAsync();
        }

        // Simple button Click event handler to take us to the second page
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}