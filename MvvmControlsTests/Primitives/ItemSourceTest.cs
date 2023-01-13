using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Tests;
using System.Linq;
using System.Windows.Controls;

namespace RFBCodeWorks.Mvvm.Primitives.Tests
{
    [TestClass()]
    public class ItemSourceTests : ControlBaseTests
    {
        public ItemSourceTests() :this(new()) { }

        /// <summary>
        /// Set the ItemSourceDefinitionTests for the test methods
        /// </summary>
        /// <param name="definition"></param>
        public ItemSourceTests(ItemSource<SelectorTestItem, SelectorTestItem[]> definition) : base(definition)
        {
            ControlDefinition = definition;
        }

        new protected ItemSource<SelectorTestItem, SelectorTestItem[]> ControlDefinition { get; }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            ControlDefinition.Items = SelectorTestItem.CreateArray();
            ControlDefinition.DisplayMemberPath = nameof(SelectorTestItem.Name);
        }

        [TestMethod]
        public void TestItemSourceChanged()
        {
            int propChanging = 0;
            int propChanged = 0;
            string propNameChanged = "";
            string propNameChanging = "";
            ControlDefinition.PropertyChanging += (o, e) =>
            {
                propChanging++;
                propNameChanging = e.PropertyName;
            };
            ControlDefinition.PropertyChanged += (o, e) =>
            {
                propChanged++;
                propNameChanged = e.PropertyName;
            };

            ControlDefinition.Items = SelectorTestItem.CreateArray();
            Assert.AreEqual(1, propChanging);
            Assert.AreEqual(1, propChanged);
            Assert.AreEqual(nameof(ControlDefinition.Items), propNameChanging);
            Assert.AreEqual(nameof(ControlDefinition.Items), propNameChanged);
        }

        [TestMethod]
        public void TestDisplaymemberPath()
        {
            int propChanging = 0;
            int propChanged = 0;
            string propNameChanged = "";
            string propNameChanging = "";
            ControlDefinition.PropertyChanging += (o, e) =>
            {
                propChanging++;
                propNameChanging = e.PropertyName;
            };
            ControlDefinition.PropertyChanged += (o, e) =>
            {
                propChanged++;
                propNameChanged = e.PropertyName;
            };

            ControlDefinition.DisplayMemberPath = "value";
            Assert.AreEqual(1, propChanging);
            Assert.AreEqual(1, propChanged);
            Assert.AreEqual(nameof(ControlDefinition.DisplayMemberPath), propNameChanging);
            Assert.AreEqual(nameof(ControlDefinition.DisplayMemberPath), propNameChanged);
        }

        protected override void TestControlInteraction(Control cntrl)
        {
            base.TestControlInteraction(cntrl);
            Assert.IsTrue(cntrl is System.Windows.Controls.ItemsControl);
            var itemCntrl = cntrl as System.Windows.Controls.ItemsControl;
            Assert.AreEqual(SelectorTestItem.CreateArray().Length, itemCntrl.Items.Count, "ItemsControl definition is not using the expected item array");
            Assert.AreEqual(ControlDefinition.DisplayMemberPath, itemCntrl.DisplayMemberPath, "DisplayMemberPath does not match the ControlDefinition");
        }
    }
}