using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Devices;
using SLARToolKit;
using System.Windows.Threading;
using System.Windows.Media.Media3D;

namespace ARChess
{
    public partial class GamePage : PhoneApplicationPage
    {
        private GrayBufferMarkerDetector arDetector;
        private byte[] buffer = null;
        private DispatcherTimer dispatcherTimer;
        private bool isDetecting;
        private bool isInitialized;
        private PhotoCamera photoCamera;
        private DetectionResult markerResult;
        private GameTimer timer;
        private ContentManager content;
        private SpriteBatch spriteBatch;
        private float aspectRatio;
        private Model myModel;
        private Vector3 modelPosition = Vector3.Zero;
        private Vector3 cameraPosition = new Vector3(0, 0, 100);

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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            isInitialized = false;
            isDetecting = false;
            SetupTheUIRenderer();

            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);
            aspectRatio = (float)SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.AspectRatio;

            //We need the Spritebatch to render the camera stream
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            //Load out 3d Model
            myModel = content.Load<Model>("saucer");

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
            var marker = Marker.LoadFromResource("resources/Marker_SLAR_16x16segments_80width.pat", 16, 16, 80);
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
            DrawModel(SharedGraphicsDeviceManager.Current);

        }

        /// <summary>
        /// Method the draws out model
        /// </summary>
        /// <param name="graphics"></param>
        protected void DrawModel(SharedGraphicsDeviceManager graphics)
        {
            if (markerResult != null) //Only draw the model if the marker is found
            {
                SharedGraphicsDeviceManager.Current.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.BlendState = BlendState.Opaque;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

                Microsoft.Xna.Framework.Matrix[] transforms = new Microsoft.Xna.Framework.Matrix[myModel.Bones.Count];
                myModel.CopyBoneTransformsTo(transforms);
                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in myModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = Microsoft.Xna.Framework.Matrix.CreateScale(0.1f) *
                            (transforms[mesh.ParentBone.Index]
                            * mesh.ParentBone.Transform
                            * Microsoft.Xna.Framework.Matrix.CreateTranslation(modelPosition)
                            * markerResult.Transformation.ToXnaMatrix()
                        );

                        effect.View = Microsoft.Xna.Framework.Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
                        effect.Projection = Microsoft.Xna.Framework.Matrix.CreatePerspectiveFieldOfView(Microsoft.Xna.Framework.MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
        }
    }
}