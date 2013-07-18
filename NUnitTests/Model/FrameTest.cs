using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffusionCurves;

namespace NUnitTests.Model
{
    ///<summary>
    ///This class tests the Frame model on every possible call.
    ///</summary>
    [TestFixture]
    class FrameTest
    {
        DiffusionCurves.Model.Frame frame;
        
        /// <summary>
        /// Set up instance
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.frame = new DiffusionCurves.Model.Frame(@"Model\test.jpg");
        }

        /// <summary>
        /// Tear down instance
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.frame = null;
        }

        /// <summary>
        /// Check imageurl field on init get and set.
        /// </summary>
        [Test]
        public void CheckImageurlInitGetSet()
        {
            //Check init with GET
            Assert.That(this.frame.ImageUrl.Equals(@"Model\test.jpg"));

            //Set
            this.frame.ImageUrl = @"Model\1\test.jpg";

            //Check set with this._
            Assert.That(this.frame.ImageUrl.Equals(@"Model\1\test.jpg"));
        }

        /// <summary>
        /// Get & Set test
        /// </summary>
        [Test]
        public void GetSetCurves()
        {
            DiffusionCurves.Model.PathContainer pathContainer = new DiffusionCurves.Model.PathContainer();
            this.frame.Curves = pathContainer;

            Assert.That(frame.Curves.Equals(pathContainer));
        }
    }
}
