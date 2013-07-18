using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiffusionCurves
{
    /// <summary>
    /// ThumbnailFrame: Defines the graphics of the thumbnailframe in the Sequencepanel.
    /// </summary>
    public partial class ThumbnailFrame : UserControl
    {
        /// <summary>
        /// Initializes ThumbnailFrame as graphical respresentation of Frame in the Sequencepanel.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        public ThumbnailFrame(string url, string name)
        {
            InitializeComponent();

            //Set image in thumbnailframe
            this.FrameImage.Source = new BitmapImage(new Uri(url));
            this.LabelName.Content = name;
        }

        /// <summary>
        /// Puts yellow border if this thumbnailframe is selected.
        /// </summary>
        public void SetToSelected()
        {
            //this.Bottom.Background = radialgradient;
            this.BottomBorder.BorderBrush = (Brush)new System.Windows.Media.BrushConverter()
                .ConvertFromString("#e67e22");
            this.BottomBorder.BorderThickness = new Thickness(0,0,0,5);
        }

        /// <summary>
        /// Puts back no border if this thumbnailframe is not selected anymore.
        /// </summary>
        public void SetToUnselected()
        {
            this.BottomBorder.BorderThickness = new Thickness(0, 0, 0, 0);
        }
    }
}
