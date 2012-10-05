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
        private int spacing_between_squares = 40;

        public ChessBoard() 
        {

        }

        /// <summary>
        /// Method the draws out model
        /// </summary>
        /// <param name="graphics"></param>
        public void Draw(SharedGraphicsDeviceManager graphics, DetectionResult markerResult, ContentManager content)
        {
            float aspectRatio = (float)graphics.GraphicsDevice.Viewport.AspectRatio;

            if (markerResult != null) //Only draw the model if the marker is found
            {
                SharedGraphicsDeviceManager.Current.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.BlendState = BlendState.Opaque;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

                //Load out 3d Model
                Model lightSquare = content.Load<Model>("light_cube");
                Model darkSquare = content.Load<Model>("dark_cube");
                Vector3 cameraPosition = new Vector3(0, 0, 150);

                for (int i = 0; i < 8; ++i)
                {
                    for (int j = 0; j < 8; ++j)
                    {
                        //generate alternating squares
                        Model myModel = (i % 2 == 0 ? (j % 2 == 0 ? darkSquare : lightSquare) : (j % 2 == 0 ? lightSquare : darkSquare));

                        Microsoft.Xna.Framework.Matrix[] transforms = new Microsoft.Xna.Framework.Matrix[myModel.Bones.Count];
                        myModel.CopyBoneTransformsTo(transforms);
                        // Draw the model. A model can have multiple meshes, so loop.
                        foreach (ModelMesh mesh in myModel.Meshes)
                        {
                            foreach (BasicEffect effect in mesh.Effects)
                            {
                                Vector3 modelPosition = new Vector3((i < 5 ? (4 - i) * -1 : i - 4) * spacing_between_squares, (j < 5 ? (4 - j) * -1 : j - 4) * spacing_between_squares, 0);

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
    }
}
