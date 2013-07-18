using DiffusionCurves.Auxillary;
using DiffusionCurves.Storage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTests.Auxillary
{
    class TestLoadSave
    {
        /// <summary>
        /// Tests ZipFolderRenameToDCIPOK: went well.
        /// </summary>
        [Test]
        public void TestZipFolderRenameToDCIPOK()
        {
            Assert.False(File.Exists(@"Model\TestZipFolderRenameToDCIPOK.dcip"));
            CreateProject.CreateFolder(@"Model\TestZipFolderRenameToDCIPOK");
            LoadSave.ZipFolderRenameToDCIP(@"Model\TestZipFolderRenameToDCIPOK");
            Assert.True(File.Exists(@"Model\TestZipFolderRenameToDCIPOK.dcip"));
            Directory.Delete(@"Model\TestZipFolderRenameToDCIPOK");
            File.Delete(@"Model\TestZipFolderRenameToDCIPOK.dcip");
        }

        /// <summary>
        /// Tests ZipFolderRenameToDCIPOK: went wrong.
        /// </summary>
        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestZipFolderRenameToDCIPNotOK()
        {
            LoadSave.ZipFolderRenameToDCIP(@"Model\±|\Project");
        }

        /// <summary>
        /// Tests DeleteOriginalFolder: went well.
        /// </summary>
        [Test]
        public void TestDeleteOriginalFolderOK()
        {
            CreateProject.CreateFolder(@"Model\TestDelete\");
            CreateProject.CreateFolder(@"Model\TestDelete\Test");

            Assert.True(Directory.Exists(@"Model\TestDelete\"));
            Assert.True(Directory.Exists(@"Model\TestDelete\Test"));

            LoadSave.DeleteOriginalFolder(@"Model\TestDelete\");

            Assert.False(Directory.Exists(@"Model\TestDelete\"));
            Assert.False(Directory.Exists(@"Model\TestDelete\Test"));
        }

        /// <summary>
        /// Test UnZipFolder: went well.
        /// </summary>
        [Test]
        public void TestUnzipFolder()
        {
            CreateProject.CreateFolder(@"Model\TestUnzip");
            LoadSave.ZipFolderRenameToDCIP(@"Model\TestUnzip");
            LoadSave.DeleteOriginalFolder(@"Model\TestUnzip");
            Assert.True(File.Exists(@"Model\TestUnzip.dcip"));

            LoadSave.UnZipFolder(@"Model\TestUnzip");
            File.Delete(@"Model\TestUnzip.dcip");
            Assert.True(Directory.Exists(@"Model\TestUnzip\"));

            LoadSave.DeleteOriginalFolder(@"Model\TestUnzip\");

        }
    }
}
