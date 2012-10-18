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

        public void Draw()
        {
            if (mDetectionResult != null )
            {
                ModelDrawer.Draw(mDetectionResult, ModelSelector.getModel(ModelSelector.Pieces.RED_SQUARE), 0, 0, 0);
            }
        }

    }
}
