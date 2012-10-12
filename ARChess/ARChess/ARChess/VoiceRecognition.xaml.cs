using System;
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

//Nuance Voice Includes
using com.nuance.nmdp.speechkit;
using com.nuance.nmdp.speechkit.oem;
using com.nuance.nmdp.speechkit.util;
using com.nuance.nmdp.speechkit.util.audio;

namespace ARChess
{
    public partial class VoiceRecognition : PhoneApplicationPage, RecognizerListener
    {
        private SpeechKit speechKit = null;
        private Recognizer recognizer = null;
        private Prompt beep = null;
        private OemConfig oemconfig = new OemConfig();
        private object handler = null;
        private PhoneApplicationPage page;
       
        public VoiceRecognition()
        {
            InitializeComponent();

            //speechkitInitialize();

            //App.CancelSpeechKit += new CancelSpeechKitEventHandler(App_CancelSpeechKit);
        }

        ~VoiceRecognition()
        {
            //speechKit.release();

            //App.CancelSpeechKit -= new CancelSpeechKitEventHandler(App_CancelSpeechKit);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                ContentTemplate = (DataTemplate)this.Resources["PivotContentTemplate"],
                LeftButtonContent = "speak",
                RightButtonContent = "read it",
                IsFullScreen = true // Pivots should always be full-screen.
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        //dictationStart(RecognizerRecognizerType.Search);
                        break;
                    case CustomMessageBoxResult.RightButton:
                        break;
                    case CustomMessageBoxResult.None:
                        break;
                    default:
                        break;
                }
            };

            messageBox.Show();
        }

        void App_CancelSpeechKit()
        {
            if (speechKit != null)
            {
                speechKit.cancelCurrent();
            }
        }

        private bool speechkitInitialize()
        {
            try
            {
                speechKit = SpeechKit.initialize(NuanceAPIKey.SpeechKitAppId, NuanceAPIKey.SpeechKitServer, NuanceAPIKey.SpeechKitPort, NuanceAPIKey.SpeechKitSsl, NuanceAPIKey.SpeechKitApplicationKey);
            }
            catch
            {
                ; //we don't care
            }

            beep = speechKit.defineAudioPrompt("resources/beep.wav");
            speechKit.setDefaultRecognizerPrompts(beep, null, null, null);
            speechKit.connect();
            Thread.Sleep(10); // to guarantee the time to load prompt resource

            return true;
        }

        public void dictationStart(string type)
        {
            Thread thread = new Thread(() =>
            {
                recognizer = speechKit.createRecognizer(type, RecognizerEndOfSpeechDetection.Long, oemconfig.defaultLanguage(), this, handler);
                recognizer.start();
            });
            thread.Start();
        }

        void dictationStop(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Content as string;

            Thread thread = new Thread(() =>
            {
                switch (content)
                {
                    case "Stop":
                        recognizer.stopRecording();

                        break;
                    case "Cancel":
                        if (recognizer != null)
                        {
                            recognizer.cancel();
                        }
                        break;
                    default:
                        break;
                }
            });
            thread.Start();
        }


        public void onRecordingBegin(Recognizer recognizer)
        {

        }

        public void onRecordingDone(Recognizer recognizer)
        {

        }

        public void onResults(Recognizer recognizer, Recognition _results)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
            
            recognizer.cancel();
            recognizer = null;
        }

        public void onError(Recognizer recognizer, SpeechError error)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));

            recognizer.cancel();
            recognizer = null;
        }
    }
}