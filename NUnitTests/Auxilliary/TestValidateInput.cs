using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DiffusionCurves.Auxillary;
using DiffusionCurves.Storage;

namespace NUnitTests.Auxillary
{
    /// <summary>
    /// This class tests the ValidateInput static class.
    /// </summary>
    [TestFixture]
    class TestValidateInput
    {
        /// <summary>
        /// Tests PathString on good path.
        /// </summary>
        [Test]
        public void TestPathStringRight()
        {
            string path = @"\Desktop";
            string filename = "test";
            string returnvalue = @"\Desktop\test";

            Assert.AreEqual(ValidateInput.PathString(filename, path), returnvalue);
        }

        /// <summary>
        /// Check ValidFilename on valid name.
        /// </summary>
        [Test]
        public void TestValidFilenameOK()
        {
            Assert.True(ValidateInput.ValidFileName("testproject.dcip"));
        }

        /// <summary>
        /// Check ValidFilename on invalid name.
        /// </summary>
        [Test]
        public void TestValidFilenameNotOK()
        {
            Assert.False(ValidateInput.ValidFileName("<.dcip"));
        }

        /// <summary>
        /// Check ValidDestination on valid name.
        /// </summary>
        [Test]
        public void TestValidDestinationOK()
        {
            Assert.True(ValidateInput.ValidDestination(@"Model"));
        }

        /// <summary>
        /// Check ValidDestination on invalid name.
        /// </summary>
        [Test]
        public void TestValidDestinationNotOK()
        {
            Assert.False(ValidateInput.ValidDestination(@"Model\±|"));
        }

        /// <summary>
        /// Check on emptyness with empty string.
        /// </summary>
        [Test]
        public void TestEmptyFilenameOK1()
        {
            Assert.True(ValidateInput.EmptyFilename(""));
        }

        /// <summary>
        /// Check on emptyness with 'name or leave empty'.
        /// </summary>
        [Test]
        public void TestEmptyFilenameOK2()
        {
            Assert.True(ValidateInput.EmptyFilename("<name or leave empty>"));
        }

        /// <summary>
        /// Check on emptyness with non empty string.
        /// </summary>
        [Test]
        public void TestEmptyFilenameNotOK()
        {
            Assert.False(ValidateInput.EmptyFilename("This is definitely not where I parked my car."));
        }

        /// <summary>
        /// Check on emptyness with empty string.
        /// </summary>
        [Test]
        public void TestEmptyDestinationOK1()
        {
            Assert.True(ValidateInput.EmptyDestination(""));
        }

        /// <summary>
        /// Check on emptyness with 'name or leave empty'.
        /// </summary>
        [Test]
        public void TestEmptyDestinationOK2()
        {
            Assert.True(ValidateInput.EmptyDestination(@"\<dest. or leave empty>"));
        }

        /// <summary>
        /// Check on emptyness with non empty string.
        /// </summary>
        [Test]
        public void TestEmptyDestinationNotOK()
        {
            Assert.False(ValidateInput.EmptyDestination("This is definitely not where I parked my car, again."));
        }


        /// <summary>
        /// Check if filename exists: exist.
        /// </summary>
        [Test]
        public void TestFilenameExistsExist()
        {
            CreateProject.CreateFolder(@"Model\exist");
            LoadSave.ZipFolderRenameToDCIP(@"Model\exist");
            Assert.True(ValidateInput.FileNameExists("exist", @"Model\"));
            LoadSave.DeleteOriginalFolder(@"Model\exist");
            System.IO.File.Delete(@"Model\exist.dcip");
        }

        /// <summary>
        /// Check if filename exists: doesn't exist.
        /// </summary>
        [Test]
        public void TestFilenameExistsNotExist()
        {
            Assert.False(ValidateInput.FileNameExists("doesntexist", @"Model\"));
        }

        /// <summary>
        /// Check ValidateAndConvertToDouble on non faulty input.
        /// </summary>
        [Test]
        public void TestValidateAndConvertToDoubleOK()
        {
            string teststring = "100";
            Assert.AreEqual(ValidateInput.ValidateAndConvertToDouble(teststring), new Tuple<bool, double>(true, 100));
        }

        /// <summary>
        /// Check ValidateAndConvertToDouble on faulty input: nondigit.
        /// </summary>
        [Test]
        public void TestValidateAndConvertToDoubleNotOK1()
        {
            string teststring = "100px";
            Assert.AreNotEqual(ValidateInput.ValidateAndConvertToDouble(teststring), new Tuple<bool, double>(true, 100));
            Assert.AreEqual(ValidateInput.ValidateAndConvertToDouble(teststring), new Tuple<bool, double>(false, Double.MinValue));
        }

        /// <summary>
        /// Check ValidateAndConvertToDouble on faulty input: negative.
        /// </summary>
        [Test]
        public void TestValidateAndConvertToDoubleNotOK2()
        {
            string teststring = "-100";
            Assert.AreNotEqual(ValidateInput.ValidateAndConvertToDouble(teststring), new Tuple<bool, double>(true, 100));
            Assert.AreEqual(ValidateInput.ValidateAndConvertToDouble(teststring), new Tuple<bool, double>(false, Double.MinValue));
        }

        /// <summary>
        /// Check ValidateAndConvertToInt on non faulty input.
        /// </summary>
        [Test]
        public void TestValidateAndConvertToInt()
        {
            string teststring = "100";
            Assert.AreEqual(ValidateInput.ValidateAndConvertToInt(teststring), new Tuple<bool, int>(true, 100));
        }
        /// <summary>
        /// Check ValidateAndConvertToInt on faulty input: nondigit.
        /// </summary>
        [Test]
        public void TestValidateAndConvertToIntNotOK1()
        {
            string teststring = "100px";
            Assert.AreNotEqual(ValidateInput.ValidateAndConvertToInt(teststring), new Tuple<bool, int>(true, 100));
            Assert.AreEqual(ValidateInput.ValidateAndConvertToInt(teststring), new Tuple<bool, int>(false, 0));
        }

        /// <summary>
        /// Check ValidateAndConvertToInt on faulty input: negative.
        /// </summary>
        [Test]
        public void TestValidateAndConvertToIntNotOK2()
        {
            string teststring = "-100";
            Assert.AreNotEqual(ValidateInput.ValidateAndConvertToInt(teststring), new Tuple<bool, int>(true, 100));
            Assert.AreEqual(ValidateInput.ValidateAndConvertToInt(teststring), new Tuple<bool, int>(false, 0));
        }

    }
}
