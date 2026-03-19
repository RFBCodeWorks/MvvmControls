using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Behaviors = RFBCodeWorks.WPF.Behaviors;
using RFBCodeWorks.Mvvm.Tests;

namespace RFBCodeWorks.Mvvm.Primitives.Tests
{

    [TestClass]
    public class ToggleButtonDefinitionTest : ControlBaseTests
    {
        public ToggleButtonDefinitionTest() : this(new()) { }

        protected ToggleButtonDefinitionTest(ToggleButtonDefinition controlDef) : base(controlDef)
        {
            ControlDefinition = controlDef;
        }

        new protected ToggleButtonDefinition ControlDefinition { get; }

        [TestMethod]
        public virtual void ModelTest_ThreeState()
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
        public virtual void ModelTest_BooleanState()
        {
            ControlDefinition.IsThreeState = false;
            ControlDefinition.IsChecked = false;
            bool CheckedEvent = false;
            bool UnCheckedEvent = false;
            bool StateChanged = false;
            bool NullEvent = false;

            ControlDefinition.Checked += (o, e) => CheckedEvent = true;
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


        protected override Control GetControl()
        {
            var cntrl = new ToggleButton();
            Behaviors.ControlDefinitions.SetToggleButtonDefinition(cntrl, ControlDefinition);
            return cntrl;
        }

        [STATestMethod]
        public void ControlTest_IsChecked()
        {
            var toggle = GetControl() as ToggleButton;
            Assert.IsNotNull(toggle);
            
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
                toggle.IsChecked = null;
                Assert.IsNull(ControlDefinition.IsChecked);
            }
        }
    }
}

