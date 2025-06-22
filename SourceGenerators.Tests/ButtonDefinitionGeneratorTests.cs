using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.SourceGenerators;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests
{
    [TestClass]
    public class ButtonDefinitionGeneratorTests
    {
        [TestMethod]
        public void When_ButtonAttributePresent_GeneratesBackingFieldAndProperty()
        {
            string input = @"
using RFBCodeWorks.Mvvm;
namespace TestNameSpace {
    partial class TestClass {
        [Button(DisplayName = ""Run"")]
        void DoSomething() { }
        bool CanDoSomething() => true;
    }
}";
            var compilation = TestHelpers.CreateCompilation(input);
            var generator = new RFBCodeWorks.Mvvm.SourceGenerators.ButtonGeneratorRoslyn311();
            var (driver, outputCompilation, diagnostics) = TestHelpers.RunGenerator(compilation, generator);

            // No generator diagnostics
            var runResult = driver.GetRunResult();
            Assert.AreEqual(0, runResult.Diagnostics.Length);

            // Should have one generated tree
            Assert.AreEqual(1, runResult.GeneratedTrees.Length);

            // Assert field and property
            TestHelpers.AssertFieldExists(outputCompilation, "TestClass", "_doSomethingButton", "global::RFBCodeWorks.Mvvm.ButtonDefinition");
            TestHelpers.AssertPropertyInitialization(outputCompilation, "TestClass", "DoSomethingButton",
                new Regex(@"_doSomethingButton\s*\?\?=\s*new\s+global::RFBCodeWorks.Mvvm.ButtonDefinition\s*\(\s*DoSomething\s*\)"));
        }
    }
}
