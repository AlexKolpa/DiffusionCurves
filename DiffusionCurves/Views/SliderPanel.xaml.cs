using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DiffusionCurves.Model;

namespace DiffusionCurves.Views
{
    /// <summary>
    /// Interaction logic for SliderPanel.xaml
    /// </summary>
    public partial class SliderPanel : TimeLinePanel
    {
        #region Private fields

        long previewInterval = 10000L;

        FramesContainer framesContainer;
        Track _track;
        DispatcherTimer previewTimer;

        Size previewSize = new Size(100, 80);

        bool isAttached;

        AdornerLayer myAdornerLayer;
        PreviewAdorner adorner;
        String oldPath = null;

        Point mousePosition;

        #endregion

        /// <summary>
        /// Constructor for Sliderpanel.
        /// </summary>
        /// <param name="container"></param>
        public SliderPanel(FramesContainer container)
        {
            InitializeComponent();
            Slider.ApplyTemplate();

            framesContainer = container;
            framesContainer.CollectionChanged += framesContainer_CollectionChanged;
            framesContainer.PropertyChanged += framesContainer_PropertyChanged;
            Slider.Value = framesContainer.CursorIndex;
            Slider.Maximum = framesContainer.Count-1;
            Slider.ValueChanged += Slider_ValueChanged;

            adorner = new PreviewAdorner(Slider, previewSize);                    
            previewTimer = new DispatcherTimer();

            previewTimer.Interval = new TimeSpan(previewInterval);
            previewTimer.Tick += previewTimer_Tick;

            _track = this.Slider.Template.FindName("PART_Track", this.Slider) as Track;
        }
        
        /// <summary>
        /// Handler for when the content of framesContainer has been changed. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void framesContainer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Slider.ValueChanged -= Slider_ValueChanged;

            Slider.Value = framesContainer.CursorIndex;

            Slider.ValueChanged += Slider_ValueChanged;
        }

        /// <summary>
        /// Handler for when the collection in framesContainer has been changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void framesContainer_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {            
            Slider.Maximum = Math.Max(framesContainer.Count-1, 0);
            if(e.NewStartingIndex >= 0)
                framesContainer.CursorIndex = e.NewStartingIndex;
        }
    
        /// <summary>
        /// Handler for when the slider value has been changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            framesContainer.CursorIndex = (int)e.NewValue;
        }

        /// <summary>
        /// Gets value from the absolute position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        double GetValueFromPosition(Point position)
        {
            return _track.ValueFromPoint(position);
        }

        /// <summary>
        /// Handler for when the mouse is pressen on the slider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (framesContainer.Count == 0)
                return;

            framesContainer.CursorIndex = Math.Max((int)GetValueFromPosition(e.GetPosition(_track)) -1, 0);
        }

        /// <summary>
        /// Handler for when the mouse has entered the slider area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isAttached)
            {
                myAdornerLayer = AdornerLayer.GetAdornerLayer(Slider);
                myAdornerLayer.Add(adorner);
                isAttached = true;
            }

            previewTimer.Start();
        }

        /// <summary>
        /// handler for when the mouse has left the slider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_MouseLeave(object sender, MouseEventArgs e)
        {
            previewTimer.Stop();
            adorner.PreviewBitmap = null;
            ResetLayer();
        }

        /// <summary>
        /// Handler for when the slider is moved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_MouseMove(object sender, MouseEventArgs e)
        {
            mousePosition = e.GetPosition(_track);
        }
        
        /// <summary>
        /// Handlers the ticker of the preview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void previewTimer_Tick(object sender, EventArgs e)
        {
            if (framesContainer.Count == 0)
                return;

            String previewBitmapPath = framesContainer.FramesList.ElementAt((int)GetValueFromPosition(mousePosition)).ImageUrl;
            
            if (oldPath != previewBitmapPath)
            {                
                BitmapImage previewImage = new BitmapImage();
                previewImage.BeginInit();
                previewImage.DecodePixelHeight = (int)previewSize.Height;
                previewImage.DecodePixelWidth = (int)previewSize.Width;
                previewImage.UriSource = new Uri(previewBitmapPath);
                previewImage.EndInit();

                adorner.PreviewBitmap = previewImage;
                ResetLayer();                
            }            

            Point previewPosition = new Point(mousePosition.X, 0);
                        
            adorner.MousePosition = previewPosition;            
                        
            ResetLayer();           
        }

        /// <summary>
        /// Resets the layer.
        /// </summary>
        void ResetLayer()
        {
            if (isAttached)
            {
                myAdornerLayer.Remove(adorner);
                myAdornerLayer.Add(adorner);
            }
        }
    }
}
