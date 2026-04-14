
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests
{
    [TestClass]
    public sealed class SourceWriterTests
    {
        [TestMethod]
        public void Test_IndentLevels()
        {
            var writer = new SourceWriter(token: default);
            Assert.IsNotNull(writer);

            writer.Indentation = 0;
            writer.Indent();
            Assert.AreEqual("", writer.ToString(), "Indentation Level 0 should not indent");
        }

        [TestMethod]
        public void Test_BlockIndentation()
        {
            var writer = new SourceWriter(token: default);

            string expected = @"
namespace TestNameSpace
{
    public partial class TestClass
    {
        public void Test()
        {
            var x = new TestClass
            (
                token: default
            )
            {
                Property = value,
                Prop2 = value2
            };
        }
    }
}
";
            writer
                .WriteLine()
                .BeginBlock("namespace TestNameSpace")
                .BeginBlock("public partial class TestClass")
                .BeginBlock("public void Test()")
                .BeginBlock("var x = new TestClass", '(', true)
                .WriteLine("token: default")
                .EndBlock(false, false)
                .BeginBlock()
                .WriteLine("Property = value,")
                .WriteLine("Prop2 = value2")
                .EndBlock(true, true);

            var result = writer.ToSourceText().ToString();
            Assert.AreEqual(expected, result);
            Console.WriteLine(result);
        }

        [TestMethod]
        public void Test_BlockIndentation2()
        {
            var writer = new SourceWriter(token: default);

            string expected = @"
namespace TestNameSpace
{
    public partial class TestClass
    {
        public void Test()
        {
            var x = new TestClass(token: default)
            {
                Property = value,
                Prop2 = value2
            };
        }
    }
}
";
            writer
                .WriteLine()
                .BeginBlock("namespace TestNameSpace")
                .BeginBlock("public partial class TestClass")
                .BeginBlock("public void Test()")
                .BeginBlock("var x = new TestClass", '(', false)
                .Write("token: default")
                .EndBlock(false, false)
                .BeginBlock()
                .WriteLine("Property = value,")
                .WriteLine("Prop2 = value2")
                .EndBlock(true, true);

            var result = writer.ToSourceText().ToString();
            Assert.AreEqual(expected, result);
            Console.WriteLine(result);
        }

        [TestMethod]
        public void Test_BlockIndentation3()
        {
            var writer = new SourceWriter(token: default);

            string expected = @"
namespace TestNameSpace
{
    public partial class TestClass
    {
        MyProp = myField ??=  var x = new TestClass
        (
            token: default
        );
    }
}
";
            writer
                .WriteLine()
                .BeginBlock("namespace TestNameSpace")
                .BeginBlock("public partial class TestClass")
                .BeginBlock("MyProp = myField ??=  var x = new TestClass", '(', true)
                .WriteLineIf(true, "token: default")
                .EndBlock(true, true);

            var result = writer.ToSourceText().ToString();
            Assert.AreEqual(expected, result);
            Console.WriteLine(result);
        }
    }
}
