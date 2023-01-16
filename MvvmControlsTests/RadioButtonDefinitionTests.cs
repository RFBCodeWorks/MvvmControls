using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using RFBCodeWorks.Mvvm.Primitives;
using RFBCodeWorks.Mvvm.Primitives.Tests;
using Behaviors = RFBCodeWorks.WPF.Behaviors;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass()]
    public class RadioButtonDefinitionTest : ToggleButtonDefinitionTest
    {
        public RadioButtonDefinitionTest() : this(new()) { }
        public RadioButtonDefinitionTest(RadioButtonDefinition controlDef) : base(controlDef)
        {
            ControlDefinition = controlDef;
        }

        new protected RadioButtonDefinition ControlDefinition { get; }

        [TestMethod]
        public override void TestToggleButton()
        {
            System.Windows.Controls.RadioButton radio = new();
            Behaviors.ControlDefinitions.SetRadioButtonDefinition(radio, ControlDefinition);
            TestControlInteraction(radio);
        }

        public override void TestThreeState()
        {
            Assert.IsFalse(ControlDefinition.IsThreeState);
            ControlDefinition.IsThreeState = true;
            Assert.IsFalse(ControlDefinition.IsThreeState);
        }

        protected override void TestControlInteraction(Control cntrl)
        {
            base.TestControlInteraction(cntrl);
            Assert.IsTrue(cntrl is RadioButton);
            var radioBtn = cntrl as RadioButton;

            ControlDefinition.GroupName = "Group1";
            Assert.AreEqual("Group1", radioBtn.GroupName);

            ControlDefinition.GroupName = "Group2";
            Assert.AreEqual("Group2", radioBtn.GroupName);
        }
    }
}