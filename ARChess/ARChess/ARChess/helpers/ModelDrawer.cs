using System;
using System.Windows;
using System.Collections.Generic;
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
    public class ModelDrawer
    {
        public static int SCALE = 31;

        public static void DrawLine(DetectionResult markerResult, Vector3 startPoint, Vector3 endPoint)
        {
            if (markerResult != null)
            {
                float aspectRatio = (float)SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.AspectRatio;

                SharedGraphicsDeviceManager.Current.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.BlendState = BlendState.Opaque;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

                Vector3 cameraPosition = new Vector3(0, 0, 150);

                BasicEffect basicEffect = new BasicEffect(SharedGraphicsDeviceManager.Current.GraphicsDevice);
                basicEffect.World = Microsoft.Xna.Framework.Matrix.CreateScale(SCALE / 2) * markerResult.Transformation.ToXnaMatrix();
                basicEffect.View = Microsoft.Xna.Framework.Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
                basicEffect.Projection = Microsoft.Xna.Framework.Matrix.CreatePerspectiveFieldOfView(Microsoft.Xna.Framework.MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);

                //SharedGraphicsDeviceManager.Current.GraphicsDevice.dr
            }
        }

        public static void Draw(DetectionResult markerResult, Model model, double x, double y, double z, float zRotation = 0.0f)
        {
            if (markerResult != null)
            {
                float aspectRatio = (float)SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.AspectRatio;

                SharedGraphicsDeviceManager.Current.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.BlendState = BlendState.Opaque;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

                Vector3 cameraPosition = new Vector3(0, 0, 150);

                Microsoft.Xna.Framework.Matrix[] transforms = new Microsoft.Xna.Framework.Matrix[model.Bones.Count];
                model.CopyBoneTransformsTo(transforms);
                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        Vector3 modelPosition = new Vector3((int)((x < 5 ? (4 - x) * -1 : x - 4) * SCALE), (int)((y < 5 ? (4 - y) * -1 : y - 4) * SCALE), (int)(z * SCALE));

                        effect.EnableDefaultLighting();
                        effect.World = Microsoft.Xna.Framework.Matrix.CreateScale(SCALE / 2) *
                            (transforms[mesh.ParentBone.Index]
                            * mesh.ParentBone.Transform
                            * Matrix.CreateRotationZ(zRotation)
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
