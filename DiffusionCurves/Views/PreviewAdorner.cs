using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DiffusionCurves.Views
{
    /// <summary>
    /// Class for defining the preview on the timeline.
    /// </summary>
    class PreviewAdorner : Adorner
    {
        #region Private fields

        double previewHeight = 80, previewWidth = 100;
        double mouseYOffset = 10;
        BitmapImage previewBitmap;
        System.Windows.Point mousePos;

        #endregion

        #region Getters setters

        public double PreviewWidth
        {
            get { return previewWidth; }
        }

        public double PreviewHeight
        {
            get { return previewHeight; }
        }

        public double MouseYOffset
        {
            get { return mouseYOffset; }
        }

        public BitmapImage PreviewBitmap
        {
            get { return previewBitmap; }
            set { previewBitmap = value; }
        }

        public System.Windows.Point MousePosition
        {
            get { return mousePos; }
            set { mousePos = value; }
        }

        #endregion

        /// <summary>
        /// Constructor for the preview.
        /// </summary>
        /// <param name="adornedElement"></param>
        public PreviewAdorner(UIElement adornedElement, System.Windows.Size size) 
            :base(adornedElement)
        {
            previewWidth = size.Width;
            previewHeight = size.Height;
        }

        /// <summary>
        /// Renders the preview image.
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            if (PreviewBitmap == null)
                return;

            Rect placement = calculateRectSize();
            
            System.Windows.Media.Pen renderPen = new System.Windows.Media.Pen(new SolidColorBrush(Colors.DarkGray), 1.5);

            drawingContext.DrawRectangle(null, renderPen, placement);
            drawingContext.DrawImage(previewBitmap, placement);
        }
    
        /// <summary>
        /// Calculates the right size for the preview.
        /// </summary>
        /// <returns></returns>
        Rect calculateRectSize()
        {
            return new Rect(MousePosition.X - previewWidth / 2, - mouseYOffset - previewHeight, previewWidth, previewHeight);
        }
    }
}
