using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffusionCurves.Import;

namespace NUnitTests.Import
{
    /// <summary>
    /// Test class for ImportImages.
    /// </summary>
    class TestImportImages
    {
        /// <summary>
        /// Tests BuildFrames on good input: right path.
        /// </summary>
        [Test]
        public void TestBuildFramesOK()
        {
            //given right filename..
            string a, b;
            a = @"Model\test.jpg";
            b = @"Model\test2.jpg";
                       
            string[] array = new string[2] { a, b };

            List<DiffusionCurves.Model.Frame> frameslist = ImportImages.BuildFrames(array, 0);

            Assert.NotNull(frameslist);
            Assert.That(frameslist.Count.Equals(2));   
        }

        /// <summary>
        /// Tests BuildFrames on faulty input: faulty path.
        /// </summary>
        [Test]
        public void TestBuildFramesNotOK()
        {
            string a, b;
            a = @"Model\test.jpg";
            b = @"Model\~||\";
            //c = "C:\\b\\";
            //d = "C:\\c\\";

            string[] array = new string[2] { a, b };

            List<DiffusionCurves.Model.Frame> frameslist = ImportImages.BuildFrames(array, 0);

            Assert.NotNull(frameslist);
            Assert.That(frameslist.Count.Equals(1));  
        }

        /// <summary>
        /// Tests CheckPath on correct path.
        /// </summary>
        [Test]
        public void TestCheckPathCorrect()
        {
            Assert.True(ImportImages.CheckPath(@"Model\"));
        }

        /// <summary>
        /// Tests CheckPath on incorrect path.
        /// </summary>
        [Test]
        public void TestCheckPathIncorrect()
        {
            Assert.False(ImportImages.CheckPath(@"\Model\±|\"));
        }

        /// <summary>
        /// Tests entire import on non good paths.
        /// </summary>
        [Test]
        public void TestImportOfImagesOK()
        {
            string a, b;
            a = @"Model\test.jpg";
            b = @"Model\test2.jpg";

            string[] array = new string[2] { a, b };

            List<DiffusionCurves.Model.Frame> frameslist = ImportImages.Import(true, array, 0);

            Assert.NotNull(frameslist);
            Assert.That(frameslist.Count.Equals(2));   
        }

        /// <summary>
        /// Tests entire import: not clicked.
        /// </summary>
        [Test]
        public void TestImportImagesNotClicked()
        {
            string a, b;
            a = @"Model\test.jpg";
            b = @"Model\test2.jpg";

            string[] array = new string[2] { a, b };

            List<DiffusionCurves.Model.Frame> frameslist = ImportImages.Import(false, array, 0);

            Assert.IsEmpty(frameslist);
        }

        /// <summary>
        /// Tests entire import on faulty paths.
        /// </summary>
        [Test]
        public void TestImportImagesNotOK()
        {
            string a, b;
            a = @"Model\test.jpg";
            b = @"Model\|~|";

            string[] array = new string[2] { a, b };

            List<DiffusionCurves.Model.Frame> frameslist = ImportImages.Import(true, array, 0);

            Assert.NotNull(frameslist);
            Assert.That(frameslist.Count.Equals(1));
        }

    }
}
