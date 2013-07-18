using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DiffusionCurves.Events;
using DiffusionCurves.Model;
using Emgu.CV;
using Emgu.CV.Structure;

namespace DiffusionCurves.Interpolation
{
    public class InterpolationControl
    {
        #region Private fields

        public event EventHandler<InterpolationArgs> NextFrameContainsCurves;

        Interpolator interpolator;
        FramesContainer frameList;

        #endregion

        /// <summary>
        /// Constructor for InterpolationControl.
        /// </summary>
        /// <param name="interpolator"></param>
        /// <param name="frameList"></param>
        public InterpolationControl(Interpolator interpolator, FramesContainer frameList)
        {
            this.interpolator = interpolator;
            this.frameList = frameList;
        }

        /// <summary>
        /// Interpolates a single frame, starting at index
        /// </summary>
        /// <param name="index">The index pointing to the first frame</param>
        public double InterpolateManual(int index, double maxError)
        {
            Frame firstFrame = frameList.FramesList.ElementAt(index);
            Frame secondFrame = frameList.FramesList.ElementAt(index + 1);

            Image<Bgr, Byte> firstImage = GetImageAt(index);
            Image<Bgr, Byte> secondImage = GetImageAt(index + 1);
            
            IPathContainer tempContainer = new PathContainer();
            double error = interpolator.InterpolatePaths(firstFrame.Curves, tempContainer, firstImage, secondImage);
            if (error < maxError)
            {
                if (secondFrame.Curves.TotalPointCount > 0)
                {
                    if (NextFrameContainsCurves != null)
                    {
                        InterpolationArgs args = new InterpolationArgs();
                        
                        NextFrameContainsCurves(this, args);                        

                        if (args.Handled == true)
                            secondFrame.Curves = tempContainer;
                        else
                            return double.MaxValue;
                    }
                    else
                        return double.MaxValue;
                }
                else
                {
                    secondFrame.Curves = tempContainer;
                }                
            }
            
            return error;
        }

        /// <summary>
        /// Returns the image at the given index in the frames container.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Image<Bgr, Byte> GetImageAt(int index)
        {
            Frame frame = frameList.FramesList[index];
            return new Image<Bgr, Byte>(frame.ImageUrl);
        }
    }
}
