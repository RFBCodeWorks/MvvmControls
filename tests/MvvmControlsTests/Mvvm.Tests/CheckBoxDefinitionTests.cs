using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using RFBCodeWorks.Mvvm.Primitives.Tests;
using Behaviors = RFBCodeWorks.WPF.Behaviors;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass]
    public class CheckBoxDefinitionTest : ToggleButtonDefinitionTest
    {
        public CheckBoxDefinitionTest() : this(new()) { }
        protected CheckBoxDefinitionTest(CheckBoxDefinition controlDef) : base(controlDef)
        {
            ControlDefinition = controlDef;
        }

        new protected CheckBoxDefinition ControlDefinition { get; }

        /// <summary>
        /// Get a new <see cref="CheckBox"/> control
        /// </summary>
        /// <returns></returns>
        protected override Control GetControl()
        {
            var cntrl = new CheckBox();
            Behaviors.ControlDefinitions.SetCheckBoxDefinition(cntrl, ControlDefinition);
            return cntrl;
        }
    }
}