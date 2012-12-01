using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//Silverlight Augmented Reality Includes
using SLARToolKit;

using ARChess.ARVR;

namespace ARChess
{
    public delegate void CancelSpeechKitEventHandler();

    public partial class GamePage : PhoneApplicationPage
    {
        private string voiceRecognitionServerIP = "http://" + "10.0.1.53" + ":62495/ARVR.svc";

        private GrayBufferMarkerDetector arDetector = null;
        private byte[] buffer = null;
        private DispatcherTimer dispatcherTimer = null;
        private bool isDetecting = false;
        private bool isInitialized = false;
        private PhotoCamera photoCamera = null;
        private GameTimer timer = null;
        private ContentManager content = null;
        private SpriteBatch spriteBatch = null;

        private CustomMessageBox messageBox;

        private static Microphone microphone = Microphone.Default;
        private static MemoryStream microphoneMemoryStream;
        private static byte[] microphoneBuffer;
        private static ARVRClient speechRecognitionClient = null;

        private UIElementRenderer uiRenderer;
        private GameResponse response;
        private bool setupFinished = false;
        private bool alreadyCalledWait = false;
        private bool alreadyCalledPromote = false;
        private bool stateHasBeenLoaded = false;

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

            if (GameState.getInstance(false) != null)
            {
                GameState.getInstance().resetTurn();
            }

            microphone.BufferDuration = TimeSpan.FromSeconds(1);
            microphoneBuffer = new byte[microphone.GetSampleSizeInBytes(microphone.BufferDuration)];

            BasicHttpBinding binding = new BasicHttpBinding() { MaxReceivedMessageSize = int.MaxValue, MaxBufferSize = int.MaxValue };
            EndpointAddress address = new EndpointAddress(voiceRecognitionServerIP);
            speechRecognitionClient = new ARVRClient(binding, address);

            microphone.BufferReady += delegate
            {
                microphone.GetData(microphoneBuffer);
                microphoneMemoryStream.Write(microphoneBuffer, 0, microphoneBuffer.Length);
            };
            speechRecognitionClient.RecognizeSpeechCompleted += new EventHandler<RecognizeSpeechCompletedEventArgs>(_client_RecognizeSpeechCompleted);
        }

        ~GamePage()
        {
            speechRecognitionClient.RecognizeSpeechCompleted -= _client_RecognizeSpeechCompleted;
            microphone.BufferReady -= null;
        }

        public void SetupPage()
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            //Run the detection separate from the update
            dispatcherTimer.Start();

            // Create a timer for this page
            timer.Start();

            if ((GameStateManager.getInstance().getGameState().black.in_check && GameStateManager.getInstance().getCurrentPlayer() == ChessPiece.Color.BLACK) || (GameStateManager.getInstance().getGameState().white.in_check && GameStateManager.getInstance().getCurrentPlayer() == ChessPiece.Color.WHITE))
            {
                handleError("You have been placed into check by your opponent.");
            }

            if (!stateHasBeenLoaded)
            {
                GameState.getInstance().loadState(GameStateManager.getInstance().getGameState());
                stateHasBeenLoaded = true;
            }
            
            //Initialize the camera
            photoCamera = new PhotoCamera();
            photoCamera.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(photoCamera_Initialized);
            ViewFinderBrush.SetSource(photoCamera);

            setupFinished = true;
        }

        void _client_RecognizeSpeechCompleted(object sender, RecognizeSpeechCompletedEventArgs e)
        {
            hidePopup();
            bool good = false;
            try
            {
                VoiceCommandFuzzyProcessing.process(e.Result);

                bool doesAPawnNeedPromotion = false;

                string color = GameStateManager.getInstance().getCurrentPlayer().ToString().ToLower();
                bool white_and_end = color == "white" && GameState.getInstance().getSelectedPiece() != null && GameState.getInstance().getSelectedPiece().getMasqueradeType() == "pawn" && GameState.getInstance().getChosenPosition().X == 7;
                bool black_and_end = color == "black" && GameState.getInstance().getSelectedPiece() != null && GameState.getInstance().getSelectedPiece().getMasqueradeType() == "pawn" && GameState.getInstance().getChosenPosition().X == 0;

                doesAPawnNeedPromotion = (white_and_end || black_and_end);

                //check to see if pawn promotion is needed
                if (doesAPawnNeedPromotion && !alreadyCalledPromote)
                {
                    showPopup("Pawn Promote");
                }
                else
                {
                    SetupPage();
                }
                good = true;
            }
            catch (Exception ex)
            {
                handleError(ex.Message);
            }
            finally
            {
                if (!good)
                {
                    SetupPage();
                }
            }
        }

        public void TeardownPage()
        {
            isInitialized = false;
            isDetecting = false;

            photoCamera.Dispose();
            photoCamera.Initialized -= photoCamera_Initialized;

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

            // Setup both markers
            Marker[] markers = GameState.getInstance().getMarkers();
            
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

                GameState.getInstance().Detect(dr);
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
            GameState.getInstance().Update();
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

            //Draw game
            try
            {
                GameState.getInstance().Draw();

                bool doesAPawnNeedPromotion = false;

                string color = GameStateManager.getInstance().getCurrentPlayer().ToString().ToLower();
                bool white_and_end = color == "white" && GameState.getInstance().getSelectedPiece() != null && GameState.getInstance().getSelectedPiece().getMasqueradeType() == "pawn" && GameState.getInstance().getChosenPosition().X == 7;
                bool black_and_end = color == "black" && GameState.getInstance().getSelectedPiece() != null && GameState.getInstance().getSelectedPiece().getMasqueradeType() == "pawn" && GameState.getInstance().getChosenPosition().X == 0;

                doesAPawnNeedPromotion = (white_and_end || black_and_end);

                //check to see if pawn promotion is needed
                if (doesAPawnNeedPromotion && !alreadyCalledPromote)
                {
                    TeardownPage();
                    showPopup("Pawn Promote");
                    alreadyCalledPromote = true;
                }
            }
            catch (Exception ex)
            {
                handleError(ex.Message);
            }

            if (GameStateManager.getInstance().getShouldWait() && setupFinished && !alreadyCalledWait)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    waitForOpponent();
                });
                alreadyCalledWait = true;
            }
        }

        private void handleError(string error)
        {
            GameState.getInstance().resetTurn();
            VibrateController vibrate = VibrateController.Default;
            vibrate.Start(TimeSpan.FromMilliseconds(2000));
            MessageBox.Show(error, "Error", MessageBoxButton.OK);
        }

        private void CommitButton_Click(object sender, EventArgs e)
        {
            
            if ( GameState.getInstance().inCheck( GameState.getInstance().getMyColor() ) )
            {
                // Self in check
                MessageBox.Show("You cannot leave your King open to attack.", "Please try another move.", MessageBoxButton.OK);

                // Reset Turn
                GameState.getInstance().resetTurn();
            }
            else
            {
                // Self not in check - Proceed
                MessageBoxResult result = MessageBox.Show("Once done, this move cannot be undone.", "Are you sure?", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    // Check is opponent is in check or checkmate
                    ChessPiece.Color opponentColor = GameState.getInstance().getMyColor() == ChessPiece.Color.BLACK ?
                        ChessPiece.Color.WHITE : ChessPiece.Color.BLACK;

                    if ( GameState.getInstance().inCheck(opponentColor) )
                    {
                        if (GameState.getInstance().checkmate(opponentColor))
                        {
                            // Checkmate
                            // Take King to signify End Game
                            //MessageBox.Show("You have placed your opponent in checkmate.", "Winner! It works!!!", MessageBoxButton.OK);
                            //pageToGo = "/WonPage.xaml";
                        }
                        else
                        {
                            // Just Check
                            // Set Check flag
                            MessageBox.Show("But can you finish him off?", "You have placed your opponent in check.", MessageBoxButton.OK);
                        }
                    }
                    // Opponent is at least in Check
                        
                    // Send result to server
                    sendMove();
                }
                else
                {
                    // Reset Turn
                    GameState.getInstance().resetTurn();
                }
            }
        }

        private void sendMove()
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (s, args) =>
            {
                new NetworkTask().sendGameState(GameState.getInstance().toCurrentGameState());
            };
            bw.RunWorkerCompleted += (s, args) =>
            {
                GameStateManager.getInstance().setShouldWait(true);
            };
            bw.RunWorkerAsync();
        }

        private void waitForOpponent()
        {
            TeardownPage();
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
                alreadyCalledWait = false;
                
                if (response.is_game_over == false)
                {
                    GameStateManager.getInstance().setShouldWait(false);
                    GameStateManager.getInstance().setGameState(response.game_state);
                    stateHasBeenLoaded = false;
                    SetupPage();
                    hidePopup();
                }
                else
                {
                    string myColor = GameState.getInstance().getMyColor() == ChessPiece.Color.BLACK ?
                        "black" : "white";

                    if (response.winner == myColor)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            NavigationService.Navigate(new Uri("/WonPage.xaml", UriKind.Relative));
                        });
                    }
                    else
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            NavigationService.Navigate(new Uri("/LostPage.xaml", UriKind.Relative));
                        });
                    }
                }
            };
            bw.RunWorkerAsync();
            showPopup("Waiting");
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
            TeardownPage();
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
                            getCommand();
                            break;
                        case CustomMessageBoxResult.RightButton:
                            AppSettings settingsUpdate = new AppSettings();
                            settingsUpdate.SpeechCommandReminderSetting = false;
                            settingsUpdate.Save();
                            getCommand();
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
                getCommand();
            }
        }

        private void showPopup(string text)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                switch (text)
                {
                    case "Pawn Promote":
                        messageBox = new CustomMessageBox()
                        {
                            ContentTemplate = (DataTemplate)this.Resources["PawnPromoteTemplate"],
                            IsLeftButtonEnabled = false,
                            IsRightButtonEnabled = false,
                            IsFullScreen = true
                        };

                        messageBox.Dismissed += (s1, e1) =>
                        {
                            switch (e1.Result)
                            {
                                case CustomMessageBoxResult.None:
                                    break;
                                default:
                                    break;
                            }
                        };
                        break;
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
                    case "Waiting":
                        messageBox = new CustomMessageBox()
                        {
                            Caption = "Please Wait",
                            Message = "Your opponent is making a move.",
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
                            LeftButtonContent = "Process",
                            IsRightButtonEnabled = false,
                            IsFullScreen = false
                        };

                        messageBox.Dismissed += (s1, e1) =>
                        {
                            switch (e1.Result)
                            {
                                case CustomMessageBoxResult.LeftButton:
                                    showPopup("Processing Dictation");
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

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void getCommand()
        {
            showPopup("Listening");

            microphone.Start();
            microphoneMemoryStream = new MemoryStream();

            var bw = new BackgroundWorker();
            bw.DoWork += (s, args) =>
            {
                Thread.Sleep(8000);
            };
            bw.RunWorkerCompleted += (s, args) =>
            {
                hidePopup();
                showPopup("Processing Dictation");
                microphone.Stop();
                speechRecognitionClient.RecognizeSpeechAsync(microphoneMemoryStream.ToArray(), microphone.SampleRate);
            };
            bw.RunWorkerAsync();
        }

        private void QueenClick(object sender, RoutedEventArgs e)
        {
            GameState.getInstance().getSelectedPiece().setMasqueradesAs("queen");
            hidePopup();
            SetupPage();
            alreadyCalledPromote = false;
        }

        private void RookClick(object sender, RoutedEventArgs e)
        {
            GameState.getInstance().getSelectedPiece().setMasqueradesAs("rook");
            hidePopup();
            SetupPage();
            alreadyCalledPromote = false;
        }

        private void BishopClick(object sender, RoutedEventArgs e)
        {
            GameState.getInstance().getSelectedPiece().setMasqueradesAs("bishop");
            hidePopup();
            SetupPage();
            alreadyCalledPromote = false;
        }

        private void KnightClick(object sender, RoutedEventArgs e)
        {
            GameState.getInstance().getSelectedPiece().setMasqueradesAs("knight");
            hidePopup();
            SetupPage();
            alreadyCalledPromote = false;
        }
    }
}