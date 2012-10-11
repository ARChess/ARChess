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

//Nuance Voice Includes
using com.nuance.nmdp.speechkit;
using com.nuance.nmdp.speechkit.oem;
using com.nuance.nmdp.speechkit.util;
using com.nuance.nmdp.speechkit.util.audio;

namespace ARChess
{
    public partial class VoiceRecognition : RecognizerListener
    {
        private SpeechKit speechKit = null;
        private Recognizer recognizer = null;
        private Prompt beep = null;
        private OemConfig oemconfig = new OemConfig();
        private object handler = null;
        private Popup popup = new Popup();
        private PhoneApplicationPage page;
        private bool haveResults = false;
        private String results;

        public VoiceRecognition(PhoneApplicationPage _page)
        {
            page = _page;
            speechkitInitialize();
        }

        public void release() 
        {
            speechKit.release();
        }

        public void cancelCurrent()
        {
            if (speechKit != null)
            {
                speechKit.cancelCurrent();
            }
        }

        public bool doesHaveResults()
        {
            return haveResults;
        }

        public string getResults()
        {
            return results;
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
            haveResults = false;
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
            popup = new Popup();
            popup.VerticalOffset = 100;
            SayACommandPopupControl control = new SayACommandPopupControl();
            popup.Child = control;
            popup.IsOpen = true;
        }

        public void onRecordingDone(Recognizer recognizer)
        {
            popup.IsOpen = false;
        }

        public void onResults(Recognizer recognizer, Recognition _results)
        {
            haveResults = true;
            results = _results.getResult(0).getText();
            recognizer.cancel();
            recognizer = null;
        }

        public void onError(Recognizer recognizer, SpeechError error)
        {
            recognizer.cancel();
            recognizer = null;
        }
    }
}
