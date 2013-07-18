using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffusionCurves;
using Rhino;
using Rhino.Mocks;
using System.ComponentModel;
using DiffusionCurves.Model;

namespace NUnitTests.Model
{
    ///<summary>
    ///This class tests the ProjectState model on every possible call.
    ///</summary>
    [TestFixture]
    class ProjectStateTest
    {
        ProjectState projectstate;
        bool called;

        /// <summary>
        /// Set up instance
        /// </summary>
        [SetUp]
        public void Setup()
        {
            called = false;
            this.projectstate = new ProjectState();
            this.projectstate.PropertyChanged += (o, e) => { called = true; };
        }

        /// <summary>
        /// Tear down instance
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.projectstate = null;
            called = false;
        }

        /// <summary>
        /// Tests if the constructor gets initialized accordingly.
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            Assert.NotNull(projectstate);
        }

        /// <summary>
        /// Tests active initialization.
        /// </summary>
        [Test]
        public void TestACTIVEInit()
        {
            Assert.False(this.projectstate.Active);
        }

        /// <summary>
        /// Tests active set and get.
        /// </summary>
        [Test]
        public void TestACTIVESetGet()
        {
            Assert.False(this.projectstate.Active = false);
        }

        /// <summary>
        /// Tests saved initialization.
        /// </summary>
        [Test]
        public void TestSAVEDInit()
        {
            Assert.False(this.projectstate.Saved);
        }

        /// <summary>
        /// Tests saved set and get.
        /// </summary>
        [Test]
        public void TestSAVEDSetGet()
        {
            //Event call setup
            bool saved = false;
            this.projectstate.PropertyChanged += (o, e) => { saved = true; };

            Assert.True(this.projectstate.Saved = true);

            //Check GUI change call
            Assert.True(saved);
        }

        /// <summary>
        /// Tests destination initialization.
        /// </summary>
        [Test]
        public void TestDESTINATIONInit()
        {
            Assert.That(this.projectstate.Destination.Equals(""));
        }

        /// <summary>
        /// Tests destination set and get.
        /// </summary>
        [Test]
        public void TestDESTINATIONSetGet()
        {
            this.projectstate.Destination = "test";
            Assert.That(this.projectstate.Destination.Equals("test"));
        }

        /// <summary>
        /// Tests filename initialization.
        /// </summary>
        [Test]
        public void TestFILENAMEInit()
        {
            Assert.That(this.projectstate.Destination.Equals(""));
        }

        /// <summary>
        /// Tests filename set and get.
        /// </summary>
        [Test]
        public void TestFILENAMESetGet()
        {
            this.projectstate.Destination = "test";
            Assert.That(this.projectstate.Destination.Equals("test"));
        }

        /// <summary>
        /// Tests width initialization.
        /// </summary>
        [Test]
        public void TestWIDTHInit()
        {
            Assert.AreEqual(this.projectstate.Width, 0);
        }

        /// <summary>
        /// Tests width set and get.
        /// </summary>
        [Test]
        public void TestWIDTHSetGet()
        {
            this.projectstate.Width = 1000;
            Assert.AreEqual(this.projectstate.Width, 1000);
        }

        /// <summary>
        /// Tests height initialization.
        /// </summary>
        [Test]
        public void TestHEIGHTInit()
        {
            Assert.AreEqual(this.projectstate.Height, 0);
        }

        /// <summary>
        /// Tests height set and get.
        /// </summary>
        [Test]
        public void TestHEIGHTSetGet()
        {
            this.projectstate.Height = 1000;
            Assert.AreEqual(this.projectstate.Height, 1000);
        }

        /// <summary>
        /// Test filename setters & getters
        /// </summary>
        [Test]
        public void FilenameSetGet()
        {
            this.projectstate.FileName = "test";

            Assert.AreEqual(projectstate.FileName, "test");
        }

        /// <summary>
        /// Test editorstate setters & getters
        /// </summary>
        [Test]
        public void EditorStateGetSetTest()
        {
            ProjectState.ProjectEditorState proj = new ProjectState.ProjectEditorState(); 
            
            this.projectstate.EditorState = proj;

            Assert.AreEqual(this.projectstate.EditorState, proj);
            Assert.False(projectstate.Saved);
            Assert.True(called);

        }

        /// <summary>
        /// Test activelayer setter & getter
        /// </summary>
        [Test]
        public void ActiveLayerIndexTest()
        {
            bool called = false;
            this.projectstate.PropertyChanged += (o, e) => { called = true; };

            this.projectstate.ActiveLayerIndex = 11;

            Assert.False(this.projectstate.Saved);
            Assert.AreEqual(11, this.projectstate.ActiveLayerIndex);
            Assert.True(called);
        }
    }
}
