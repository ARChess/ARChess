using System;
using System.Windows.Media.Media3D;

namespace ARChess
{
    public static class Matrix3dExtensions
    {
        public static Microsoft.Xna.Framework.Matrix ToXnaMatrix(this Matrix3D matrix3d)
        {
            return new Microsoft.Xna.Framework.Matrix(
                        (float)matrix3d.M11,
                        (float)matrix3d.M12,
                        (float)matrix3d.M13,
                        (float)matrix3d.M14,
                        (float)matrix3d.M21,
                        (float)matrix3d.M22,
                        (float)matrix3d.M23,
                        (float)matrix3d.M24,
                        (float)matrix3d.M31,
                        (float)matrix3d.M32,
                        (float)matrix3d.M33,
                        (float)matrix3d.M34,
                        (float)matrix3d.OffsetX,
                        (float)matrix3d.OffsetY,
                        (float)matrix3d.OffsetZ,
                        (float)matrix3d.M44);

        }
    }
}
