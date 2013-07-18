using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace DiffusionCurves.Model
{
    /// <summary>
    /// Container for a bezier point in a bezier path. 
    /// The control points should be set relative to the position.
    /// </summary>
    [Serializable]
    public class BezierPoint : INotifyPropertyChanged
    {
        #region Private fields

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        Vector2 position;

        #endregion

        #region Getters Setters.

        [XmlElement("position")]
        public Vector2 Position
        {
            get { return position; }
            set
            {
                if (position == value)
                    return;
                position = value;
                NotifyPropertyChanged();
            }
        }
        Vector2 control1, control2;

        [XmlElement("control2")]
        public Vector2 Control2
        {
            get { return control2; }
            set {
                if (control2 == value)
                    return;
                control2 = value;
                NotifyPropertyChanged();
            }
        }

        [XmlElement("control1")]
        public Vector2 Control1
        {
            get { return control1; }
            set {
                if (control1 == value)
                    return;
                control1 = value;
                NotifyPropertyChanged();
            }
        }

        Vector4 leftColor, rightColor;

        [XmlElement("rightcolor")]
        public Vector4 RightColor
        {
            get { return rightColor; }
            set { rightColor = value;
            NotifyPropertyChanged();
            }
        }

        [XmlElement("leftcolor")]
        public Vector4 LeftColor
        {
            get { return leftColor; }
            set { leftColor = value;
            NotifyPropertyChanged();
            }
        }

        #endregion

        /// <summary>
        /// Constructs the Bezier point with control points zero relative to the position 
        /// and black colors
        /// </summary>
        /// <param name="position">World position of the BezierPoint</param>
        public BezierPoint(Vector2 position)
        {
            this.position = position;
            this.control1 = new Vector2();
            this.control2 = new Vector2();
            this.leftColor = Vector4.UnitW;
            this.rightColor = Vector4.UnitW;
        }

        /// <summary>
        /// Constructs the Bezier point with control points zero relative to the position
        /// </summary>
        /// <param name="position">World position of the BezierPoint</param>
        /// <param name="controlPoint1">Control point relative to point's position</param>
        /// <param name="controlPoint2">Control point relative to point's position</param>
        public BezierPoint(Vector2 position, Vector2 controlPoint1, Vector2 controlPoint2)
        {
            this.position = position;
            this.control1 = controlPoint1;
            this.control2 = controlPoint2;
            this.leftColor = Vector4.UnitW;
            this.rightColor = Vector4.UnitW;
        }

        /// <summary>
        /// Constructs the Bezier point with control points zero relative to the position
        /// </summary>
        /// <param name="position">World position of the BezierPoint</param>
        /// <param name="controlPoint1">Control point relative to point's position</param>
        /// <param name="controlPoint2">Control point relative to point's position</param>
        /// <param name="leftColor">Color to left side of point's position 
        /// (oriented vertically)</param>
        /// <param name="rightColor">Color to right side of point's position 
        /// (oriented vertically)</param>
        public BezierPoint(Vector2 position, Vector2 controlPoint1, Vector2 controlPoint2, 
            Vector4 leftColor, Vector4 rightColor)
        {
            this.position = position;
            this.control1 = controlPoint1;
            this.control2 = controlPoint2;
            this.leftColor = leftColor;
            this.rightColor = rightColor;
        }

        /// <summary>
        /// Notifies other classes of change in values.
        /// </summary>
        /// <param name="propertyName"></param>
        void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Checks equality of bezier points.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (!(obj is BezierPoint))
                return false;

            BezierPoint point = obj as BezierPoint;

            return point.Position.Equals(this.Position) && 
                point.Control1.Equals(this.Control1) && 
                point.Control2.Equals(this.Control2) &&
                point.LeftColor.Equals(this.LeftColor) &&
                point.RightColor.Equals(this.RightColor);
        }

        /// <summary>
        /// Clones the bezier point.
        /// </summary>
        /// <returns></returns>
        public BezierPoint Clone()
        {
            BezierPoint point = new BezierPoint(position, control1, control2);
            point.LeftColor = this.LeftColor;
            point.RightColor = this.RightColor;
            return point;
        }
    }
}
