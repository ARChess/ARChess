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

//Silverlight Augmented Reality Includes
using SLARToolKit;

//Nuance Voice Includes
using com.nuance.nmdp.speechkit;
using com.nuance.nmdp.speechkit.oem;
using com.nuance.nmdp.speechkit.util;
using com.nuance.nmdp.speechkit.util.audio;

namespace ARChess
{
    public delegate void CancelSpeechKitEventHandler();

    public partial class GamePage : PhoneApplicationPage, RecognizerListener
    {
        private GrayBufferMarkerDetector arDetector = null;
        private byte[] buffer = null;
        private DispatcherTimer dispatcherTimer = null;
        private bool isDetecting = false;
        private bool isInitialized = false;
        private PhotoCamera photoCamera = null;
        private DetectionResult markerResult = null;
        private GameTimer timer = null;
        private ContentManager content = null;
        private SpriteBatch spriteBatch = null;
        private SpeechKit speechKit = null;
        private Recognizer recognizer = null;
        private Prompt beep = null;
        private OemConfig oemconfig = new OemConfig();
        private object handler = null;
        Popup popup = new Popup();

        //This is the secret sauce, this will render the Silverlight content
        private UIElementRenderer uiRenderer;

        public GamePage()
        {
            InitializeComponent();

            // Get the application's ContentManager
            content = (Application.Current as App).Content;

            // Create a timer for this page
            timer = new GameTimer { UpdateInterval = TimeSpan.FromTicks(333333) };
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            speechkitInitialize();

            App.CancelSpeechKit += new CancelSpeechKitEventHandler(App_CancelSpeechKit);
        }

        ~GamePage()
        {
            speechKit.release();

            App.CancelSpeechKit -= new CancelSpeechKitEventHandler(App_CancelSpeechKit);
        }

        void App_CancelSpeechKit()
        {
            Logger.info(this, "App_CancelSpeechKit()");
            if (speechKit != null)
            {
                speechKit.cancelCurrent();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            isInitialized = false;
            isDetecting = false;
            SetupTheUIRenderer();

            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            //We need the Spritebatch to render the camera stream
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            //Initialize the camera
            photoCamera = new PhotoCamera();
            photoCamera.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(photoCamera_Initialized);
            ViewFinderBrush.SetSource(photoCamera);
            // Start the timer
            timer.Start();

            //Runt the detection separate from the update
            dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            dispatcherTimer.Tick += (sender, e1) => Detect();
            dispatcherTimer.Start();


            base.OnNavigatedTo(e);
        }

        private void SetupTheUIRenderer()
        {
            // Do we already have a UIElementRenderer of the correct size?
            if (uiRenderer != null &&
                uiRenderer.Texture != null &&
                uiRenderer.Texture.Width == 800 &&
                uiRenderer.Texture.Height == 480)
            {
                return;
            }

            // Before constructing a new UIElementRenderer, be sure to Dispose the old one
            if (uiRenderer != null)
                uiRenderer.Dispose();

            // Create the renderer
            uiRenderer = new UIElementRenderer(this, 800, 480);
        }

        void photoCamera_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            // Initialize the Detector
            //This need to be done AFTER the camera is initialized
            arDetector = new GrayBufferMarkerDetector();
            var marker = Marker.LoadFromResource("resources/marker.pat", 16, 16, 80);
            arDetector.Initialize(System.Convert.ToInt32(photoCamera.PreviewResolution.Width), System.Convert.ToInt32(photoCamera.PreviewResolution.Height), 1, 4000, marker);
            isInitialized = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }
        private void Detect()
        {
            //Here is where we try to detect the marker
            if (isDetecting || !isInitialized)
            {
                return;
            }

            isDetecting = true;

            try
            {
                // Update buffer size
                var pixelWidth = photoCamera.PreviewResolution.Width;
                var pixelHeight = photoCamera.PreviewResolution.Height;
                if (buffer == null || buffer.Length != pixelWidth * pixelHeight)
                {
                    buffer = new byte[System.Convert.ToInt32(pixelWidth * pixelHeight)];
                }

                // Grab snapshot for the marker detection
                photoCamera.GetPreviewBufferY(buffer);

                //Detect the markers
                arDetector.Threshold = 100;
                var dr = arDetector.DetectAllMarkers(buffer, System.Convert.ToInt32(pixelWidth), System.Convert.ToInt32(pixelHeight));

                //Set the marker result if the marker is found
                if (dr.HasResults)
                {
                    markerResult = dr[0];
                }
                else
                {
                    markerResult = null;
                }
            }
            finally
            {
                isDetecting = false;
            }
        }


        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            //We don't need to update anything

        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            //Renders your Silverlight content
            uiRenderer.Render();
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            //Draw your ui renderer onto a texture feed texture
            spriteBatch.Begin();
            spriteBatch.Draw(uiRenderer.Texture, Vector2.Zero, Microsoft.Xna.Framework.Color.White);
            spriteBatch.End();

            //Draw our model
            new ChessBoard(content).Draw(SharedGraphicsDeviceManager.Current, markerResult);

        }

        private void ResignButton_Click(object sender, EventArgs e)
        {

        }

        private void VoiceCommandButton_Click(object sender, EventArgs e)
        {
            dictationStart(RecognizerRecognizerType.Search);
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

        private void dictationStart(string type)
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

        public void onResults(Recognizer recognizer, Recognition results)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                
            });
            recognizer.cancel();
            recognizer = null;
        }

        public void onError(Recognizer recognizer, SpeechError error)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(error.getErrorDetail());
            });
            recognizer.cancel();
            recognizer = null;
        }
    }
}