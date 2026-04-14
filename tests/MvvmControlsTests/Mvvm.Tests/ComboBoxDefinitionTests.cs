using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using RFBCodeWorks.Mvvm.Primitives.Tests;
using Behaviors = RFBCodeWorks.WPF.Behaviors;
using System.Windows.Controls.Primitives;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass]
    public class ComboBoxDefinitionTests : IComboBoxTests<ComboBoxDefinition<SelectorTestItem, SelectorTestItem[], string>> 
    { 
        public ComboBoxDefinitionTests() : base(new()) { } 
    }

    /// <summary>
    /// Abstract test class for <see cref="IComboBox"/> objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class IComboBoxTests<T> : SelectorTests 
        where T : class, IComboBox<SelectorTestItem>, ISelector<SelectorTestItem>
    {
        public IComboBoxTests(T definition) : base(definition)
        {
            ControlDefinition = definition;
        }

        new protected IComboBox<SelectorTestItem> ControlDefinition { get; }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
        }

        /// <summary>
        /// Returns a <see cref="ComboBox"/>
        /// </summary>
        protected override Selector GetSelector()
        {
            var cmb = new ComboBox();
            cmb.SetValue(Behaviors.ControlDefinitions.ComboBoxDefinitionProperty, ControlDefinition);
            return cmb;
        }

        [STATestMethod]
        public void ControlTest_IsDropDownOpen()
        {
            var cmb = GetSelector() as ComboBox;
            Assert.IsNotNull(cmb);

            cmb.IsDropDownOpen = true;
            Assert.IsTrue(ControlDefinition.IsDropDownOpen);
            ControlDefinition.IsDropDownOpen = false;
            Assert.IsFalse(cmb.IsDropDownOpen);
        }
    }
}