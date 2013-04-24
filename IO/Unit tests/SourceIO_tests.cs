using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AEGIScript.IO.Unit_tests
{
    [TestFixture]
    class SourceIO_tests
    {

        public List<String> TestContent1 { get; set; }

        [SetUp]
        public void Init()
        {
            #region Setting up test lists
            TestContent1 = new List<string>();
            TestContent1.Add("First line");
            TestContent1.Add("Second line");
            #endregion
        }

        [Test]
        [TestCase("TestFiles/test1.aes")]
        [TestCase("TestFiles/test2.aes")]
        public void IsValidPath_Valid_Test(String Path)
        {
            Assert.IsTrue(SourceIO.isValidPath(Path));
        }

        [Test]
        [TestCase("TestFiles/test1aes.s")]
        [TestCase("TestFiles/test2.abs")]
        [TestCase("TestFiles/1.aes")]
        [TestCase(".aes")]
        public void IsValidPath_Invalid_Test(String Path)
        {
            Assert.IsFalse(SourceIO.isValidPath(Path));
        }

    }
}
