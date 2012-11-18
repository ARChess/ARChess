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
        //private DetectionResult markerResult = null;
        private GameTimer timer = null;
        private ContentManager content = null;
        private SpriteBatch spriteBatch = null;

        private SpeechKit speechKit = null;
        private Recognizer recognizer = null;
        private Prompt beep = null;
        private OemConfig oemconfig = new OemConfig();
        private object handler = null;
        private string ttsText = string.Empty;
        private string ttsVoice = string.Empty;
        private CustomMessageBox messageBox;

        //This is the secret sauce, this will render the Silverlight content
        private UIElementRenderer uiRenderer;

        //private PieceSelector selector;
        private GameState gameState;

        public GamePage()
        {
            InitializeComponent();

            // Get the application's ContentManager
            content = (Application.Current as App).Content;

            timer = new GameTimer { UpdateInterval = TimeSpan.FromTicks(333333) };
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            dispatcherTimer.Tick += (sender, e1) => Detect();
            
            speechkitInitialize();

            App.CancelSpeechKit += new CancelSpeechKitEventHandler(App_CancelSpeechKit);
        }

        ~GamePage()
        {
            speechKit.release();

            App.CancelSpeechKit -= new CancelSpeechKitEventHandler(App_CancelSpeechKit);
        }

        public void SetupPage()
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            //Run the detection separate from the update
            dispatcherTimer.Start();

            // Create a timer for this page
            timer.Start();

            GameState.getInstance().loadState(GameStateManager.getInstance().getGameState());
            gameState = GameState.getInstance();
        }

        public void TeardownPage()
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            isInitialized = false;
            isDetecting = false;
            SetupTheUIRenderer();

            SetupPage();

            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            //Initialize the camera
            photoCamera = new PhotoCamera();
            photoCamera.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(photoCamera_Initialized);
            ViewFinderBrush.SetSource(photoCamera);
           
            base.OnNavigatedTo(e);
        }

        void App_CancelSpeechKit()
        {
            if (speechKit != null)
            {
                speechKit.cancelCurrent();
                hidePopup();
            }
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

            // Setup both markers
            // I believe the string name (final parameter), is needed to differentiate the markers during detection
            //Marker boardMarker = Marker.LoadFromResource("resources/marker.pat", 16, 16, 80, "board_marker");
            Marker[] markers = gameState.getMarkers();//{ boardMarker, selector.getMarker() };
            
            arDetector.Initialize(System.Convert.ToInt32(photoCamera.PreviewResolution.Width), System.Convert.ToInt32(photoCamera.PreviewResolution.Height), 1, 4000, markers);
            isInitialized = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            TeardownPage();

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
                DetectionResults dr = arDetector.DetectAllMarkers(buffer, System.Convert.ToInt32(pixelWidth), System.Convert.ToInt32(pixelHeight));

                gameState.Detect(dr);
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
            gameState.Update();
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

            //Draw Board
            //new ChessBoard(content).Draw(markerResult);
            //Draw Pieces
            gameState.Draw();
            // Draw selector
            //selector.Draw();
        }

        // TODO: This is a temporary function to test hardcoded piece movement
        private void MoveForwardButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Once done, this move cannot be undone.", "Are you sure?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                var bw = new BackgroundWorker();
                bw.DoWork += (s, args) =>
                {
                    ChessPiece piece = gameState.getSelectedPiece();
                    if (piece != null)
                    {
                        Vector2 currentPosition = piece.getPosition();
                        if (currentPosition != null)
                        {
                            //currentPosition.Y = 2;
                            currentPosition.X += 1;
                            gameState.movePiece(currentPosition);
                        }
                    }

                    //send to central server
                    new NetworkTask().sendGameState(GameState.getInstance().toCurrentGameState());
                };
                bw.RunWorkerCompleted += (s, args) =>
                {
                    NavigationService.Navigate(new Uri("/WaitForOpponentPage.xaml", UriKind.Relative));
                };
                bw.RunWorkerAsync();
            }
        }

        private void ResignButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("You are about to surrender to your opponent.  Once done, this cannot be undone.", "Are you sure?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                var bw = new BackgroundWorker();
                bw.DoWork += (s, args) =>
                {
                    new NetworkTask().resignGame();
                };
                bw.RunWorkerCompleted += (s, args) =>
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                };
                bw.RunWorkerAsync();
            }
        }

        private void VoiceCommandButton_Click(object sender, EventArgs e)
        {
            /*TeardownPage();
            AppSettings settings = new AppSettings();

            if (settings.SpeechCommandReminderSetting)
            {
                messageBox = new CustomMessageBox()
                {
                    ContentTemplate = (DataTemplate)this.Resources["PivotContentTemplate"],
                    LeftButtonContent = "Speak",
                    RightButtonContent = "Don't Show",
                    IsFullScreen = true // Pivots should always be full-screen.
                };

                messageBox.Dismissed += (s1, e1) =>
                {
                    switch (e1.Result)
                    {
                        case CustomMessageBoxResult.LeftButton:
                            dictationStart(RecognizerRecognizerType.Search);
                            break;
                        case CustomMessageBoxResult.RightButton:
                            AppSettings settingsUpdate = new AppSettings();
                            settingsUpdate.SpeechCommandReminderSetting = false;
                            settingsUpdate.Save();
                            dictationStart(RecognizerRecognizerType.Search);
                            break;
                        case CustomMessageBoxResult.None:
                            break;
                        default:
                            break;
                    }
                };

                messageBox.Show();
            }
            else
            {
                dictationStart(RecognizerRecognizerType.Search);
            }*/
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }


        private bool speechkitInitialize()
        {
            try
            {
                speechKit = SpeechKit.initialize(NuanceAPIKey.SpeechKitAppId, NuanceAPIKey.SpeechKitServer, NuanceAPIKey.SpeechKitPort, NuanceAPIKey.SpeechKitSsl, NuanceAPIKey.SpeechKitApplicationKey);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                return false;
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
                showPopup("Please wait");
            });
            thread.Start();
        }

        private void showPopup(string text)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                switch (text)
                {
                    case "Processing Dictation":
                        messageBox = new CustomMessageBox()
                        {
                            Caption = "Processing",
                            Message = "We're processing your command.  Please wait.",
                            IsLeftButtonEnabled = false,
                            IsRightButtonEnabled = false,
                            IsFullScreen = false
                        };
                        break;
                    case "Listening":
                        messageBox = new CustomMessageBox()
                        {
                            Caption = "Say a Command",
                            Message = "We're listening.",
                            LeftButtonContent = "Stop",
                            RightButtonContent = "Cancel",
                            IsFullScreen = false
                        };

                        messageBox.Dismissed += (s1, e1) =>
                        {
                            switch (e1.Result)
                            {
                                case CustomMessageBoxResult.LeftButton:
                                    recognizer.stopRecording();
                                    showPopup("Processing Dictation");
                                    break;
                                case CustomMessageBoxResult.RightButton:
                                    if (recognizer != null)
                                    {
                                        recognizer.cancel();
                                    }
                                    hidePopup();
                                    break;
                                case CustomMessageBoxResult.None:
                                    break;
                                default:
                                    break;
                            }
                        };
                        break;
                    default:
                        break;
                }
            });

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                messageBox.Show();
            });
        }

        private void hidePopup()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                messageBox.Dismiss();
            });
        }

        public void onRecordingBegin(Recognizer recognizer)
        {
            showPopup("Listening");
        }

        public void onRecordingDone(Recognizer recognizer)
        {
            showPopup("Processing Dictation");
        }

        public void onResults(Recognizer recognizer, Recognition results)
        {
            hidePopup();
            recognizer.cancel();
            recognizer = null;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                SetupPage();
            });
        }

        public void onError(Recognizer recognizer, SpeechError error)
        {
            hidePopup();
            recognizer.cancel();
            recognizer = null;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                SetupPage();
            });
        }
    }
}