using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using RFBCodeWorks.Mvvm.Primitives.Tests;
using Behaviors = RFBCodeWorks.WPF.Behaviors;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass]
    public class RadioButtonDefinitionTest : ToggleButtonDefinitionTest
    {
        public RadioButtonDefinitionTest() : this(new()) { }
        protected RadioButtonDefinitionTest(RadioButtonDefinition controlDef) : base(controlDef)
        {
            ControlDefinition = controlDef;
        }

        new protected RadioButtonDefinition ControlDefinition { get; }

        protected override Control GetControl()
        {
            var cntrl = new RadioButton();
            Behaviors.ControlDefinitions.SetRadioButtonDefinition(cntrl, ControlDefinition);
            return cntrl;
        }

        public override void ModelTest_ThreeState()
        {
            Assert.IsFalse(ControlDefinition.IsThreeState);
            ControlDefinition.IsThreeState = true;
            Assert.IsFalse(ControlDefinition.IsThreeState);
        }

        [STATestMethod]
        public void ControlTest_GroupName()
        {
            var radioBtn = GetControl() as RadioButton;
            Assert.IsNotNull(radioBtn);

            ControlDefinition.GroupName = "Group1";
            Assert.AreEqual("Group1", radioBtn.GroupName);

            ControlDefinition.GroupName = "Group2";
            Assert.AreEqual("Group2", radioBtn.GroupName);
        }
    }
}