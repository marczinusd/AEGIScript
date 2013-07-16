using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AEGIScript.IO.Unit_tests
{
    [TestFixture]
    class SourceIOTests
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
        [TestCase("TestFiles/constructors.aes")]
        [TestCase("TestFiles/factory.aes")]
        public void IsValidPath_Valid_Test(String path)
        {
            Assert.IsTrue(SourceIO.IsValidPath(path));
        }

        [Test]
        [TestCase("TestFiles/test1aes.s")]
        [TestCase("TestFiles/test2.abs")]
        [TestCase("TestFiles/1.aes")]
        [TestCase(".aes")]
        public void IsValidPath_Invalid_Test(String path)
        {
            Assert.IsFalse(SourceIO.IsValidPath(path));
        }

    }
}
