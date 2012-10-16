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
    public class ChessBoard
    {
        private int scale = 31;
        private ContentManager content;

        public ChessBoard(ContentManager _content) 
        {
            content = _content;
        }

        private Model selectModel(int curX, int curY)
        {
            Model lightCube = content.Load<Model>("light_cube");
            Model darkCube = content.Load<Model>("dark_cube");

            if (curX % 2 == 0)
            {
                if (curY % 2 == 0)
                {
                    return lightCube;
                }
                else
                {
                    return darkCube;
                }
            }
            else
            {
                if (curY % 2 == 0)
                {
                    return darkCube;
                }
                else
                {
                    return lightCube;
                }
            }

            return null;
        }

        /// <summary>
        /// Method the draws out model
        /// </summary>
        /// <param name="graphics"></param>
        public void Draw(SharedGraphicsDeviceManager graphics, DetectionResult markerResult)
        {
            float aspectRatio = (float)graphics.GraphicsDevice.Viewport.AspectRatio;

            if (markerResult != null) //Only draw the model if the marker is found
            {
                SharedGraphicsDeviceManager.Current.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.BlendState = BlendState.Opaque;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
                
                Vector3 cameraPosition = new Vector3(0, 0, 150);

                for (int i = 0; i < 8; ++i)
                {
                    for (int j = 0; j < 8; ++j)
                    {
                        //generate alternating squares
                        Model myModel = selectModel(j, i);

                        Microsoft.Xna.Framework.Matrix[] transforms = new Microsoft.Xna.Framework.Matrix[myModel.Bones.Count];
                        myModel.CopyBoneTransformsTo(transforms);
                        // Draw the model. A model can have multiple meshes, so loop.
                        foreach (ModelMesh mesh in myModel.Meshes)
                        {
                            foreach (BasicEffect effect in mesh.Effects)
                            {
                                Vector3 modelPosition = new Vector3((i < 5 ? (4 - i) * -1 : i - 4) * scale, (j < 5 ? (4 - j) * -1 : j - 4) * scale, 0);

                                effect.EnableDefaultLighting();
                                effect.World = Microsoft.Xna.Framework.Matrix.CreateScale(scale/2) *
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
    }
}
