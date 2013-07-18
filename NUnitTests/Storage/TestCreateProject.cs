using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using DiffusionCurves.Storage;
using DiffusionCurves.Auxillary;

namespace NUnitTests.Storage
{
    class TestCreateProject
    {
        //No setup, no teardown

        /// <summary>
        /// Tests CreateFolder: went well.
        /// </summary>
        [Test]
        public void TestCreateFolderOK()
        {
            CreateProject.CreateFolder(@"Model\TestCreateFolder");
            Assert.True(Directory.Exists(@"Model\TestCreateFolder"));
            Directory.Delete(@"Model\TestCreateFolder");
        }

        /// <summary>
        /// Tests CreateFolder: went wrong.
        /// </summary>
        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestCreateFolderNotOK()
        {
            CreateProject.CreateFolder(@"\Model\±|");
        }

        /// <summary>
        /// Tests Create full path: went well.
        /// </summary>
        [Test]
        public void TestCreateOK()
        {
            CreateProject.Create("testproject", @"Model\TestCreate\");
            LoadSave.DeleteOriginalFolder(@"Model\TestCreate\");
        }

        /// <summary>
        /// Tests Create full path: no write permission (System32 folder).
        /// </summary>
        [Test]
        [ExpectedException("System.UnauthorizedAccessException")]
        public void TestCreateNoPermission32()
        {
            CreateProject.Create("testproject", @"C:\Windows\System32\TestProject");
        }
    }
}
