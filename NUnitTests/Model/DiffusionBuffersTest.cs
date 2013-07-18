using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DiffusionCurves.Diffusion;
using OpenTK;
using DiffusionCurves.Model;

namespace NUnitTests.Model
{
    /// <summary>
    /// Testclass for the DiffusionBuffers class
    /// </summary>
    [TestFixture]
    class DiffusionBuffersTest
    {
        DiffusionBuffers diffbuff;

        /// <summary>
        /// Set up instance
        /// </summary>
        [SetUp]
        public void SetUp() 
        {
            this.diffbuff = new DiffusionBuffers();
        }

        /// <summary>
        /// Tear down instance after every test
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.diffbuff = null;
        }

        /// <summary>
        /// Test correct initialisation by constructor
        /// </summary>
        [Test]
        public void DiffusionBuffersContructorTest()
        {
            Assert.NotNull(diffbuff);
            Assert.NotNull(diffbuff.ListPathPoints);
            Assert.NotNull(diffbuff.ListLeftColorPoints);
            Assert.NotNull(diffbuff.ListRightColorPoints);
            Assert.NotNull(diffbuff.ListIndices);
            Assert.NotNull(diffbuff.PathPoints);
        }

        /// <summary>
        /// Get & Set test for PathPoints
        /// </summary>
        [Test]
        public void PathPointsTest()
        {
            Vector3[] pathPoints = new Vector3[1];
            pathPoints[0] = new Vector3(123, 342, 356);
            diffbuff.PathPoints = pathPoints;

            Assert.That(diffbuff.PathPoints[0].Equals(pathPoints[0]));
        }
        
        /// <summary>
        /// Get & Set test for ListPathPoints
        /// </summary>
        [Test]
        public void ListPathPointsTest()
        {
            List<Vector3[]> listPathPoints = new  List<Vector3[]>();
            Vector3[] vectorArray = new Vector3[1];
            vectorArray[0] = new Vector3(123, 342, 356);
            listPathPoints.Add(vectorArray);
            diffbuff.ListPathPoints = listPathPoints;

            Assert.That(diffbuff.ListPathPoints.ElementAt(0)[0].Equals(listPathPoints.ElementAt(0)[0]));
        }

        /// <summary>
        /// Get & Set test for ListLeftColorPoints
        /// </summary>
        [Test]
        public void ListLeftColorPointsTest()
        {
            List<Vector4[]> listLeftColorPoints = new List<Vector4[]>();
            Vector4[] vectorArray = new Vector4[1];
            vectorArray[0] = new Vector4(123, 342, 356, 480);
            listLeftColorPoints.Add(vectorArray);
            diffbuff.ListLeftColorPoints = listLeftColorPoints;

            Assert.That(diffbuff.ListLeftColorPoints.ElementAt(0)[0].Equals(listLeftColorPoints.ElementAt(0)[0]));
        }

        /// <summary>
        /// Get & Set test for ListRightColorPoints
        /// </summary>
        [Test]
        public void ListRightColorPointsTest()
        {
            List<Vector4[]> listRightColorPoints = new List<Vector4[]>();
            Vector4[] vectorArray = new Vector4[1];
            vectorArray[0] = new Vector4(123, 342, 356, 480);
            listRightColorPoints.Add(vectorArray);
            diffbuff.ListRightColorPoints = listRightColorPoints;

            Assert.That(diffbuff.ListRightColorPoints.ElementAt(0)[0].Equals(listRightColorPoints.ElementAt(0)[0]));
        }

        /// <summary>
        /// Get & Set test for ListIndices
        /// </summary>
        [Test]
        public void ListIndicesTest()
        {
            List<uint[]> listIndices = new List<uint[]>();
            uint[] array = new uint[1];
            array[0] = 48;
            listIndices.Add(array);
            diffbuff.ListIndices = listIndices;

            Assert.That(diffbuff.ListIndices.ElementAt(0)[0].Equals(48));
        }
    }
}
