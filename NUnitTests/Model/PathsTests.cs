using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DiffusionCurves.Diffusion;
using OpenTK;
using DiffusionCurves.Model;

namespace NUnitTests.GUI
{
    /// <summary>
    /// Tet class for the Paths class
    /// </summary>
    [TestFixture]
    class PathsTests
    {
        Path path;
        BezierPoint pointOne1, pointOne2, pointThree1, pointThree2;
        Vector2 vector1, vector2, vector3;

        /// <summary>
        /// Set up instance
        /// </summary>
        [SetUp]
        public void PathsConstructorTest()
        {
            vector1 = new Vector2(100, 100);
            vector2 = new Vector2(50, 50);
            vector3 = new Vector2(100, 50);

            pointOne1 = new BezierPoint(vector1);
            pointOne2 = new BezierPoint(vector2);

            pointThree1 = new BezierPoint(vector1, vector2, vector3);
            pointThree2 = new BezierPoint(vector3, vector2, vector1);


            path = new Path();
        }

        /// <summary>
        /// Tear down instance
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            path = null;

            vector1 = new Vector2(-1,-1);
            vector2 = new Vector2(-1, -1);

            pointOne1 = null;
            pointOne2 = null;

            pointThree1 = null;
            pointThree2 = null;
        }

        /// <summary>
        /// Test constructor
        /// </summary>
        [Test]
        public void ConstructorTest()
        {
            Assert.IsNotNull(path);
            Assert.That(path.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Test AddPointLast functionality
        /// </summary>
        [Test]
        public void AddPointLastTest()
        {
            path.AddPointLast(vector1);

            Assert.That(path.GetLastPoint(), Is.EqualTo(pointOne1));
            Assert.That(path.GetFirstPoint(), Is.EqualTo(pointOne1));
            Assert.That(path.Count, Is.EqualTo(1));
            
            path.AddPointLast(vector2);

            Assert.That(path.GetLastPoint(), Is.EqualTo(pointOne2));
            Assert.That(path.GetFirstPoint(), Is.EqualTo(pointOne1));
            Assert.That(path.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Test AddPointFirst functionality
        /// </summary>
        [Test]
        public void AddPointFirstTest()
        {
            Assert.That(path.Count, Is.EqualTo(0));

            path.AddPointFirst(vector1);

            Assert.That(path.GetFirstPoint(), Is.EqualTo(pointOne1));
            Assert.That(path.GetLastPoint(), Is.EqualTo(pointOne1));
            Assert.That(path.Count, Is.EqualTo(1));

            path.AddPointFirst(vector2);

            Assert.That(path.GetFirstPoint(), Is.EqualTo(pointOne2));
            Assert.That(path.GetLastPoint(), Is.EqualTo(pointOne1));
            Assert.That(path.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Test AddPoint functionality
        /// </summary>
        [Test]
        public void AddPointTest()
        {
            Assert.That(path.Count, Is.EqualTo(0));

            path.AddPoint(vector1, vector2, vector3);

            Assert.That(path.Count, Is.EqualTo(1));
            Assert.That(path.GetLastPoint(), Is.EqualTo(pointThree1));
            Assert.That(path.GetFirstPoint(), Is.EqualTo(pointThree1));

            path.AddPoint(vector3, vector2, vector1);

            Assert.That(path.Count, Is.EqualTo(2));
            Assert.That(path.GetLastPoint(), Is.EqualTo(pointThree2));
            Assert.That(path.GetFirstPoint(), Is.EqualTo(pointThree1));
        }
       
        /// <summary>
        /// Test removing a non existent point
        /// </summary>
        [Test]
        public void RemovePointTest_NonExistentPoint()
        {
            Assert.That(path.Count, Is.EqualTo(0));

            path.AddPoint(vector1, vector2, vector3);
            Assert.That(path.Count, Is.EqualTo(1));
            
            path.RemovePoint(pointThree2);
            Assert.That(path.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Test RemovePoint functionality with one point
        /// </summary>
        [Test]
        public void RemovePointTest_OnePoint()
        {
            Assert.That(path.Count, Is.EqualTo(0));
            path.AddPoint(vector1, vector2, vector3);
            Assert.That(path.Count, Is.EqualTo(1));
            path.RemovePoint(pointThree1);
            Assert.That(path.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Test RemovePoint functionality with two points
        /// </summary>
        [Test]
        public void RemovePointTest_TwoPoints()
        {
            Assert.That(path.Count, Is.EqualTo(0));

            path.AddPoint(vector1, vector2, vector3);
            Assert.That(path.Count, Is.EqualTo(1));
            
            path.AddPoint(vector3, vector2, vector1);
            Assert.That(path.Count, Is.EqualTo(2));
            
            path.RemovePoint(pointThree1);
            Assert.That(path.Count, Is.EqualTo(1));
            
            path.RemovePoint(pointThree2);
            Assert.That(path.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Test Get when path is empty
        /// </summary>
        [Test]
        public void GetPathTest_NoNodes()
        {
            LinkedList<BezierPoint> list = path.Points;
            Assert.IsNotNull(list);
            Assert.That(list.Count(), Is.EqualTo(0));
        }

        /// <summary>
        /// Test Get when path is not empty
        /// </summary>
        [Test]
        public void GetPathTest_TwoNodes()
        {
            path.AddPointFirst(vector1);
            path.AddPointFirst(vector2);

            LinkedList<BezierPoint> list = path.Points;

            Assert.IsNotNull(list);
            Assert.That(list.Count(), Is.EqualTo(2));
        }

        /// <summary>
        /// Test GetLastPoint functionality with empty path
        /// </summary>
        [Test]
        public void GetLastPointTest_EmptyPath()
        {
            Path path = new Path();
            Assert.That(path.GetLastPoint(), Is.EqualTo(null));
        }

        /// <summary>
        /// Test GetLastPoint functionality with two points
        /// </summary>
        [Test]
        public void GetLastPointTest_TwoPoints()
        {
            path.AddPoint(vector1, vector2, vector3);
            path.AddPoint(vector3, vector2, vector1);
            Assert.That(path.GetLastPoint(), Is.EqualTo(pointThree2));
        }

        /// <summary>
        /// Test GetFirstPoint functionality with empty path
        /// </summary>
        [Test]
        public void GetFirstPointTest_EmptyPath()
        {
            Assert.That(path.GetFirstPoint(), Is.EqualTo(null));
        }

        /// <summary>
        /// Test GetFirstPoint functionality with two points
        /// </summary>
        [Test]
        public void GetFirstPointTest()
        {
            path.AddPoint(vector1, vector2, vector3);
            path.AddPoint(vector3, vector2, vector1);
            Assert.That(path.GetFirstPoint(), Is.EqualTo(pointThree1));
        }

        /// <summary>
        /// Test RemoveLastPoint functionality, emptypath
        /// </summary>
        [Test]
        public void RemoveLastPointTest_EmptyPath()
        {
            Assert.That(path.Count, Is.EqualTo(0));
            
            path.RemoveLastPoint();
            Assert.That(path.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Test RemoveLastPoint functionality, two points
        /// </summary>
        [Test]
        public void RemoveLastPointTest_TwoPoints()
        {
            path.AddPoint(vector1, vector2, vector3);
            path.AddPoint(vector3, vector2, vector1);

            Assert.That(path.Count, Is.EqualTo(2));

            path.RemoveLastPoint();

            Assert.That(path.Count, Is.EqualTo(1));
            Assert.That(path.GetLastPoint(), Is.EqualTo(pointThree1));
        }

        /// <summary>
        /// Test Contains functionality, true
        /// </summary>
        [Test]
        public void ContainsTest_True()
        {
            path.AddPointFirst(vector1);

            Assert.True(path.Contains(pointOne1));
        }

        /// <summary>
        /// Test Contains functionality, false
        /// </summary>
        [Test]
        public void ContainsTest_False()
        {
            path.AddPointFirst(vector1);
            Assert.False(path.Contains(pointOne2));
        }

        /// <summary>
        /// Test IsEmpty functionality, true
        /// </summary>
        [Test]
        public void IsEmptyTest_True()
        {
            Path path = new Path();
            Assert.True(path.IsEmpty());
        }

        /// <summary>
        /// Test IsEmpty functionality, false
        /// </summary>
        [Test]
        public void IsEmptyTest_False()
        {
            path.AddPointFirst(vector1);
            Assert.False(path.IsEmpty());
        }

        /// <summary>
        /// Test GetClosestPoint functionality
        /// </summary>
        [Test]
        public void GetClosestPointTest()
        {
            Vector2 vector1 = new Vector2(0, 0);
            Vector2 vector2 = new Vector2(100, 100);
            Vector2 vector3 = new Vector2(34, 67);
            BezierPoint point1 = new BezierPoint(vector1);
            BezierPoint point2 = new BezierPoint(vector2);
            Path path = new Path();
            path.AddPointFirst(vector2);
            path.AddPointFirst(vector1);
            path.AddPointFirst(vector3);
            Assert.That(point1, Is.EqualTo(path.GetClosestPoint(new Vector2(1, 1))));
            Assert.False(point2.Equals(path.GetClosestPoint(new Vector2(1, 1))));
        }

        /// <summary>
        /// Clone should copy the elements, but not the object
        /// </summary>
        [Test]
        public void TestPathClone()
        {
            Vector2 vector1 = new Vector2(0, 0);
            Vector2 vector2 = new Vector2(100, 100);
            Vector2 vector3 = new Vector2(34, 67);
            
            Path path1 = new Path();
            path1.AddPointLast(vector1);
            path1.AddPointLast(vector2);
            path1.AddPointLast(vector3);

            Path path2 = path1.Clone();
            Assert.That(path1, Is.EqualTo(path2));
            Assert.False(path1 == path2);
        }

        /// <summary>
        /// Test Count functionality
        /// </summary>
        [Test]
        public void GetLengthTest()
        {
            Assert.That(path.Count, Is.EqualTo(0));

            path.AddPointFirst(vector1);
            Assert.That(path.Count, Is.EqualTo(1));
            
            path.AddPointFirst(vector2);
            Assert.That(path.Count, Is.EqualTo(2));
        }
        
        /// <summary>
        /// Test Setters & Getters
        /// </summary>
        [Test]
        public void SetGetCurves()
        {
            LinkedList<BezierPoint> curvesList = new LinkedList<BezierPoint>();

            path.Points = curvesList;

            Assert.NotNull(path.Points);
        }

        /// <summary>
        /// Test Equals functionality with untrue objects
        /// </summary>
        [Test]
        public void EqualsFalse()
        {
            Assert.False(path.Equals(null));
            Assert.False(path.Equals(148));
            Assert.False(path.Equals("diffusion"));
        }
    }
}
