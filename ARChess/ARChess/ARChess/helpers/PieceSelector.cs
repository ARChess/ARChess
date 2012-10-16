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
using SLARToolKit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ARChess.helpers
{
    public class PieceSelector
    {
        private ContentManager content;
        private Marker mMarker;
        private Model mModel;
        private DetectionResult mDetectionResult;

        public PieceSelector(ContentManager _content)
        {
            content = _content;
            //mMarker = Marker.LoadFromResource("resources/marker.pat", 16, 16, 80);
            //mModel = _content.Load<Model>("red_cube");
        }

        public Marker getMarker()
        {
            return Marker.LoadFromResource("resources/selection_marker.pat", 16, 16, 80, "selection_marker");
        }

        public void setDetectionResult(DetectionResult result)
        {
            mDetectionResult = result;
        }

        public void Draw(SharedGraphicsDeviceManager graphics)
        {
            
            float aspectRatio = (float)graphics.GraphicsDevice.Viewport.AspectRatio;

            if (mDetectionResult != null )
            {
                mModel = content.Load<Model>("red_cube");

                SharedGraphicsDeviceManager.Current.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.BlendState = BlendState.Opaque;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

                Vector3 cameraPosition = new Vector3(0, 0, 150);

                Microsoft.Xna.Framework.Matrix[] transforms = new Microsoft.Xna.Framework.Matrix[mModel.Bones.Count];
                mModel.CopyBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in mModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        Vector3 modelPosition = new Vector3(0, 0, 0);

                        effect.EnableDefaultLighting();
                        effect.World = Microsoft.Xna.Framework.Matrix.CreateScale(30) *
                                    (transforms[mesh.ParentBone.Index]
                                    * mesh.ParentBone.Transform
                                    * Microsoft.Xna.Framework.Matrix.CreateTranslation(modelPosition)
                                    * mDetectionResult.Transformation.ToXnaMatrix()
                                );

                        effect.View = Microsoft.Xna.Framework.Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
                        effect.Projection = Microsoft.Xna.Framework.Matrix.CreatePerspectiveFieldOfView(Microsoft.Xna.Framework.MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);

                    }
                    mesh.Draw();
                }

            }
        }

    }
}
