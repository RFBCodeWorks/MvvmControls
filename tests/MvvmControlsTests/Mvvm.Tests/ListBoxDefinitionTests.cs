using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using RFBCodeWorks.Mvvm.Primitives.Tests;
using Behaviors = RFBCodeWorks.WPF.Behaviors;
using System.Windows.Controls.Primitives;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass]
    public class ListBoxDefinitionTests : SelectorTests
    {
        public ListBoxDefinitionTests() : this(new()) { }

        /// <summary>
        /// Set the ItemSourceDefinitionTests for the test methods
        /// </summary>
        /// <param name="definition"></param>
        protected ListBoxDefinitionTests(ListBoxDefinition<SelectorTestItem, SelectorTestItem[], string> definition) : base(definition)
        {
            ControlDefinition = definition;
        }

        new protected ListBoxDefinition<SelectorTestItem, SelectorTestItem[], string> ControlDefinition { get; }

        /// <summary>
        /// Returns a <see cref="ListBox"/>
        /// </summary>
        protected override Selector GetSelector()
        {
            var cmb = new ListBox();
            cmb.SetValue(Behaviors.ControlDefinitions.ListBoxDefinitionProperty, ControlDefinition);
            return cmb;
        }

        [STATestMethod]
        public void ControlTest_SelectionMode()
        {
            var listbox = GetSelector() as ListBox;
            Assert.IsNotNull(listbox);

            //SelectionMode is ONE-WAY BOUND by the behavior - ControlModel -> Control, not the other way around
            ControlDefinition.SelectionMode = SelectionMode.Single;
            Assert.AreEqual(SelectionMode.Single, listbox.SelectionMode);
            ControlDefinition.SelectionMode = SelectionMode.Multiple;
            Assert.AreEqual(SelectionMode.Multiple, listbox.SelectionMode);
            ControlDefinition.SelectionMode = SelectionMode.Extended;
            Assert.AreEqual(SelectionMode.Extended, listbox.SelectionMode);

        }
    }
}