using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.MvvmControls.Tests
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
            System.Windows.Controls.RadioButton radio= new();
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

    [TestClass()]
    public class ToggleButtonDefinitionTest : BaseControlDefinitionTests
    {
        public ToggleButtonDefinitionTest() : this(new()) { }

        public ToggleButtonDefinitionTest(ToggleButtonDefinition controlDef) : base(controlDef) 
        {
            ControlDefinition = controlDef;
        }

        new protected ToggleButtonDefinition ControlDefinition { get; }

        [TestMethod]
        public virtual void TestThreeState()
        {
            ControlDefinition.IsThreeState = true;
            ControlDefinition.IsChecked = false;

            bool StateChanged = false;
            bool NullEvent = false;
            ControlDefinition.Indeterminate += (o, e) => NullEvent = true;
            ControlDefinition.StateChange += (o, e) => StateChanged = true;
            ControlDefinition.IsChecked = null;

            Assert.IsTrue(NullEvent);
            Assert.IsTrue(StateChanged);
        }

        [TestMethod]
        public virtual void TestBooleanState()
        {
            ControlDefinition.IsThreeState = false;
            ControlDefinition.IsChecked = false;
            bool CheckedEvent = false;
            bool UnCheckedEvent = false;
            bool StateChanged = false;
            bool NullEvent = false;
            
            ControlDefinition.Checked+= (o, e) => CheckedEvent= true;
            ControlDefinition.Unchecked += (o, e) => UnCheckedEvent = true;
            ControlDefinition.Indeterminate += (o, e) => NullEvent = true;
            ControlDefinition.StateChange += (o, e) => StateChanged = true;

            ControlDefinition.IsChecked = true;
            Assert.IsTrue(CheckedEvent); CheckedEvent = false;
            Assert.IsTrue(StateChanged); StateChanged = false;
            Assert.IsFalse(UnCheckedEvent);
            Assert.IsFalse(NullEvent);

            ControlDefinition.IsChecked = false;
            Assert.IsTrue(UnCheckedEvent); CheckedEvent = false;
            Assert.IsTrue(StateChanged); StateChanged = false;
            Assert.IsFalse(CheckedEvent);
            Assert.IsFalse(NullEvent);

        }

        [TestMethod]
        public virtual void TestToggleButton()
        {
            System.Windows.Controls.Primitives.ToggleButton toggle = new();
            Behaviors.ControlDefinitions.SetToggleButtonDefinition(toggle, ControlDefinition);
            TestControlInteraction(toggle);
        }
        
        protected override void TestControlInteraction(Control cntrl)
        {
            Assert.IsTrue(cntrl is System.Windows.Controls.Primitives.ToggleButton);
            var toggle = cntrl as System.Windows.Controls.Primitives.ToggleButton;
            base.TestControlInteraction(toggle);

            ControlDefinition.IsChecked = false;
            Assert.IsFalse(toggle.IsChecked);

            //From Model
            ControlDefinition.IsChecked = true;
            Assert.IsTrue(toggle.IsChecked);
            ControlDefinition.IsChecked = false;
            Assert.IsFalse(toggle.IsChecked);
            if (ControlDefinition.IsThreeState)
            {
                ControlDefinition.IsChecked = null;
                Assert.IsNull(toggle.IsChecked);
            }

            //From Control
            toggle.IsChecked = true;
            Assert.IsTrue(ControlDefinition.IsChecked);
            toggle.IsChecked = false;
            Assert.IsFalse(ControlDefinition.IsChecked);
            if (ControlDefinition.IsThreeState)
            {
                toggle.IsChecked= null;
                Assert.IsNull(ControlDefinition.IsChecked);
            }
        }
    }
}
