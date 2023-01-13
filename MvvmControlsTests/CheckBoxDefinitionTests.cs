using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using RFBCodeWorks.Mvvvm.Primitives;
using RFBCodeWorks.Mvvvm.Primitives.Tests;
using Behaviors = RFBCodeWorks.WPFBehaviors;

namespace RFBCodeWorks.Mvvvm.Tests
{
    [TestClass()]
    public class CheckBoxDefinitionTest : ToggleButtonDefinitionTest
    {
        public CheckBoxDefinitionTest() : this(new()) { }
        public CheckBoxDefinitionTest(CheckBoxDefinition controlDef) : base(controlDef)
        {
            ControlDefinition = controlDef;
        }

        new protected CheckBoxDefinition ControlDefinition { get; }

        [TestMethod]
        public override void TestToggleButton()
        {
            System.Windows.Controls.CheckBox chk = new();
            Behaviors.ControlDefinitions.SetCheckBoxDefinition(chk, ControlDefinition);
            TestControlInteraction(chk);
        }

        protected override void TestControlInteraction(Control cntrl)
        {
            base.TestControlInteraction(cntrl);
            Assert.IsTrue(cntrl is CheckBox);
            var checkbox = cntrl as CheckBox;
        }
    }

}