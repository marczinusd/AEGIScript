using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AEGIScript.IO;
using NUnit.Framework;

namespace AEGIScript.GUI.Model.Unit_tests
{
    [TestFixture]
    class InterpreterTests
    {
        public InterpreterTests()
        {
            Init();
        }

        [SetUp]
        public void Init()
        {
            _interpreter = new Interpreter();
        }


        [Test]
        [TestCase("../../TestCode/Syntax/1.aes")]
        [TestCase("../../TestCode/Syntax/2.aes")]
        [TestCase("../../TestCode/Syntax/3.aes")]
        [TestCase("../../TestCode/Syntax/4.aes")]
        [TestCase("../../TestCode/Syntax/5.aes")]
        [TestCase("../../TestCode/Syntax/6.aes")]
        [TestCase("../../TestCode/Syntax/7.aes")]
        [TestCase("../../TestCode/Syntax/8.aes")]
        public void SyntaxErrorTests_Failing(string path)
        {
            StringBuilder builder = new StringBuilder();
            SourceIO.ReadFromFile(path).ForEach(x => builder.Append(x));
            _interpreter.Walk(builder.ToString());
            Assert.IsTrue(_interpreter.Output.ToString().Contains("Syntax"));
        }

        [Test]
        [TestCase("../../TestCode/Language/1.aes")]
        [TestCase("../../TestCode/Language/2.aes")]
        [TestCase("../../TestCode/Language/3.aes")]
        [TestCase("../../TestCode/Language/4.aes")]
        [TestCase("../../TestCode/Language/5.aes")]
        [TestCase("../../TestCode/Language/6.aes")]
        [TestCase("../../TestCode/Language/7.aes")]
        public void LanguageTests_Successful(string path)
        {
            StringBuilder builder = new StringBuilder();
            SourceIO.ReadFromFile(path).ForEach( x => builder.Append(x));
            _interpreter.Walk(builder.ToString());
            Assert.IsTrue(_interpreter.Output.ToString().Contains("Successful"));
        }

        [Test]
        [TestCase("../../TestCode/Semantic/1.aes")]
        [TestCase("../../TestCode/Semantic/2.aes")]
        [TestCase("../../TestCode/Semantic/3.aes")]
        [TestCase("../../TestCode/Semantic/4.aes")]
        [TestCase("../../TestCode/Semantic/5.aes")]
        [TestCase("../../TestCode/Semantic/6.aes")]
        [TestCase("../../TestCode/Semantic/7.aes")]
        [TestCase("../../TestCode/Semantic/8.aes")]
        public void SemanticTests_Failing(string path)
        {
            Assert.IsTrue(Cond(path, x => x.ToUpper().Contains("RUNTIME ERROR") || x.ToUpper().Contains("OUT OF RANGE")));
        }

        // AEGIS tests not working, because all of them use the builtin Dir function, which refers to a different assembly
        // when called from unit tests
        //[Test]
        //[TestCase("../../TestCode/AEGIS/shapefile.aes")]
        //[TestCase("../../TestCode/AEGIS/tif.aes")]
        //[TestCase("../../TestCode/AEGIS/wkttest.aes")]
        //public void AEGISTests_Successful(string path)
        //{
        //    Assert.IsTrue(Cond(path, x => x.Contains("Successful")));
        //}

        public bool Cond(string path, Func<string, bool> cond)
        {
            StringBuilder builder = new StringBuilder();
            SourceIO.ReadFromFile(path).ForEach(x => builder.AppendLine(x));
            _interpreter.Walk(builder.ToString());
            return cond(_interpreter.Output.ToString());
        }

        //private CancellationToken _ctok;
        private Interpreter _interpreter;
        //private AsyncOperation _operation;

    }
}
