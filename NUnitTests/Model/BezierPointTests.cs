using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DiffusionCurves.Diffusion;
using OpenTK;
using DiffusionCurves.Model;

namespace NUnitTests.Diffusion
{
    /// <summary>
    /// Testclass for the BezierPoint class
    /// </summary>
    [TestFixture]
    class BezierPointTests
    {
        /// <summary>
        /// Single argument should set controls to zero-vector
        /// </summary>
        [Test]
        public void TestConstructorOneArgument()
        {
            Vector2 testVector = new Vector2(100, 150);
            Vector2 zeroVector = new Vector2(0, 0);
            BezierPoint point = new BezierPoint(testVector);

            Assert.IsNotNull(point);
            Assert.That(point.Position, Is.EqualTo(testVector));
            Assert.That(point.Control1, Is.EqualTo(zeroVector));
            Assert.That(point.Control2, Is.EqualTo(zeroVector));
        }

        /// <summary>
        /// Triple argument should set control points and position to the given arguments
        /// </summary>
        [Test]
        public void TestConstructorThreeArguments()
        {
            Vector2 testVector = new Vector2(100, 150);
            Vector2 control1 = new Vector2(50, -50);
            Vector2 control2 = new Vector2(10, 20);
            BezierPoint point = new BezierPoint(testVector,control1, control2);

            Assert.IsNotNull(point);
            Assert.That(point.Position, Is.EqualTo(testVector));
            Assert.That(point.Control1, Is.EqualTo(control1));
            Assert.That(point.Control2, Is.EqualTo(control2));
        }

        /// <summary>
        ///  Should set controlpoints, position and color to given values.
        /// </summary>
        [Test]
        public void TestConstructorFiveArguments()
        {
            Vector2 testVector = new Vector2(100, 150);
            Vector2 control1 = new Vector2(50, -50);
            Vector2 control2 = new Vector2(10, 20);
            Vector4 color1 = new Vector4(1, 1, 1, 1);
            Vector4 color2 = new Vector4(0, 0, 0, 0);
            BezierPoint point = new BezierPoint(testVector, control1, control2, color1, color2);

            Assert.IsNotNull(point);
            Assert.That(point.Position, Is.EqualTo(testVector));
            Assert.That(point.Control1, Is.EqualTo(control1));
            Assert.That(point.Control2, Is.EqualTo(control2));
            Assert.That(point.LeftColor, Is.EqualTo(color1));
            Assert.That(point.RightColor, Is.EqualTo(color2));
        }

        /// <summary>
        /// Setter test for LeftColor.
        /// </summary>
        [Test]
        public void TestSetLeftColor()
        {
            Vector2 testVector = new Vector2(100, 150);
            Vector2 control1 = new Vector2(50, -50);
            Vector2 control2 = new Vector2(10, 20);
            Vector4 color1 = new Vector4(1, 1, 1, 1);
            Vector4 color2 = new Vector4(0, 0, 0, 0);
            BezierPoint point = new BezierPoint(testVector, control1, control2, color1, color2);

            Assert.IsNotNull(point);
            Assert.That(point.LeftColor, Is.EqualTo(color1));

            point.LeftColor = color2;

            Assert.That(point.LeftColor, Is.EqualTo(color2));
        }

        /// <summary>
        /// Setter test for RightColor.
        /// </summary>
        [Test]
        public void TestSetRightColor()
        {
            Vector2 testVector = new Vector2(100, 150);
            Vector2 control1 = new Vector2(50, -50);
            Vector2 control2 = new Vector2(10, 20);
            Vector4 color1 = new Vector4(1, 1, 1, 1);
            Vector4 color2 = new Vector4(0, 0, 0, 0);
            BezierPoint point = new BezierPoint(testVector, control1, control2, color1, color2);

            Assert.IsNotNull(point);
            Assert.That(point.RightColor, Is.EqualTo(color2));

            point.RightColor = color1;

            Assert.That(point.RightColor, Is.EqualTo(color1));
        }

        /// <summary>
        /// Setter test for control1
        /// </summary>
        [Test]
        public void TestControl1Setter()
        {
            BezierPoint point = new BezierPoint(new Vector2());
            Vector2 newControlPosition = new Vector2(20, 10);

            Assert.That(point.Control1, Is.EqualTo(new Vector2()));

            point.Control1 = newControlPosition;

            Assert.That(point.Control1, Is.EqualTo(newControlPosition));
        }

        /// <summary>
        /// Setter test for control2
        /// </summary>
        [Test]
        public void TestControl2Setter()
        {
            BezierPoint point = new BezierPoint(new Vector2());
            Vector2 newControlPosition = new Vector2(10, 20);

            Assert.That(point.Control2, Is.EqualTo(new Vector2()));

            point.Control2 = newControlPosition;

            Assert.That(point.Control2, Is.EqualTo(newControlPosition));
        }

        /// <summary>
        /// Setter test for position
        /// </summary>
        [Test]
        public void TestPositionSetter()
        {
            BezierPoint point = new BezierPoint(new Vector2());
            Vector2 newPosition = new Vector2(50, 20);

            Assert.That(point.Position, Is.EqualTo(new Vector2()));

            point.Position = newPosition;

            Assert.That(point.Position, Is.EqualTo(newPosition));
        }

        /// <summary>
        /// Calling equals on same object should return true
        /// </summary>
        [Test]
        public void TestIsEqualSelf()
        {
            BezierPoint point = new BezierPoint(
                new Vector2(10, 20), 
                new Vector2(30, 40), 
                new Vector2(50, 60));

            Assert.True(point.Equals(point));
        }

        /// <summary>
        /// Calling equals on different object but on the same position should return true
        /// </summary>
        [Test]
        public void TestIsEqualOther()
        {
            Vector2 position = new Vector2(10, 20);
            Vector2 control1 = new Vector2(30, 40);
            Vector2 control2 = new Vector2(50, 60);

            BezierPoint point1 = new BezierPoint(
                position,
                control1,
                control2);

            BezierPoint point2 = new BezierPoint(
                position,
                control1,
                control2);

            Assert.True(point1.Equals(point2));
        }

        /// <summary>
        /// BezierPoint should never equal an object of a different type
        /// </summary>
        [Test]
        public void TestIsNotEqualDifferentType()
        {
            BezierPoint point = new BezierPoint(new Vector2());
            Object randomObject = new Object();

            Assert.False(point.Equals(randomObject));
        }

        /// <summary>
        /// Two different objects with different positions are not equal
        /// </summary>
        [Test]
        public void TestIsNotEqualSameType()
        {

            BezierPoint point1 = new BezierPoint(
                 new Vector2(10, 20),
                 new Vector2(30, 40),
                 new Vector2(50, 60));

            BezierPoint point2 = new BezierPoint(
                new Vector2(60, 50),
                new Vector2(40, 30),
                new Vector2(20, 10));

            Assert.False(point1.Equals(point2));
        }

        /// <summary>
        /// Cloning should copy the values, but not the object
        /// </summary>
        [Test]
        public void TestClone()
        {
            BezierPoint point1 = new BezierPoint(
                  new Vector2(10, 20),
                  new Vector2(30, 40),
                  new Vector2(50, 60));

            BezierPoint point2 = point1.Clone();

            Assert.That(point1, Is.EqualTo(point2));
            Assert.False(point1 == point2);
        }

        /// <summary>
        /// Tests PropertyChanged events in different conditions
        /// </summary>
        #region PropertyChangedTests
        [Test]
        public void TestNotifyPropertyChangedPosition()
        {
            bool hasChanged = false;
            BezierPoint point = new BezierPoint(new Vector2());
            point.PropertyChanged += (o,e) => {hasChanged = true; };

            point.Position = new Vector2(10, 10);
            Assert.True(hasChanged);
        }

        [Test]
        public void TestNotifyPropertyChangedPositionWithoutChange()
        {
            bool hasChanged = false;
            BezierPoint point = new BezierPoint(new Vector2());
            point.PropertyChanged += (o, e) => { hasChanged = true; };

            point.Position = new Vector2();
            Assert.False(hasChanged);
        }

        [Test]
        public void TestNotifyPropertyChangedControl1()
        {
            bool hasChanged = false;
            BezierPoint point = new BezierPoint(new Vector2());
            point.PropertyChanged += (o, e) => { hasChanged = true; };

            point.Control1 = new Vector2(10, 10);
            Assert.True(hasChanged);
        }

        [Test]
        public void TestNotifyPropertyChangedControl1WithoutChange()
        {
            bool hasChanged = false;
            BezierPoint point = new BezierPoint(new Vector2());
            point.PropertyChanged += (o, e) => { hasChanged = true; };

            point.Control1 = new Vector2();
            Assert.False(hasChanged);
        }

        [Test]
        public void TestNotifyPropertyChangedControl2()
        {
            bool hasChanged = false;
            BezierPoint point = new BezierPoint(new Vector2());
            point.PropertyChanged += (o, e) => { hasChanged = true; };

            point.Control2 = new Vector2(10, 10);
            Assert.True(hasChanged);
        }

        [Test]
        public void TestNotifyPropertyChangedControl2WithoutChange()
        {
            bool hasChanged = false;
            BezierPoint point = new BezierPoint(new Vector2());
            point.PropertyChanged += (o, e) => { hasChanged = true; };

            point.Control2 = new Vector2();
            Assert.False(hasChanged);
        }
        #endregion
    }
}
