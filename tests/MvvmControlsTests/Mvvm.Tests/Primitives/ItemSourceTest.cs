
using RFBCodeWorks.Mvvm.Tests;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RFBCodeWorks.Mvvm.Primitives.Tests
{
    [TestClass]
    public class ItemSourceTests : ControlBaseTests
    {

        public ItemSourceTests() :this(new())  { }

        /// <summary>
        /// Set the ItemSourceDefinitionTests for the test methods
        /// </summary>
        /// <param name="definition"></param>
        protected ItemSourceTests(ItemSource<SelectorTestItem, SelectorTestItem[]> definition) : base(definition)
        {
            ControlDefinition = definition;
        }

        new protected ItemSource<SelectorTestItem, SelectorTestItem[]> ControlDefinition { get; }

        /// <summary>
        /// Returns a new <see cref="ItemsControl"/>
        /// </summary>
        protected virtual ItemsControl GetItemsControl()
        {
            var cntrl = new ItemsControl();
            RFBCodeWorks.WPF.Behaviors.ControlDefinitions.BindItemSource(cntrl, ControlDefinition);
            return cntrl;
        }

        /// <summary>Returns the result of <see cref="GetItemsControl"/></summary>
        /// <returns>Some type of <see cref="ItemsControl"/></returns>
        protected sealed override Control GetControl() => GetItemsControl();

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            ControlDefinition.Items = SelectorTestItem.CreateArray();
            ControlDefinition.DisplayMemberPath = nameof(SelectorTestItem.Name);
        }

        [TestMethod]
        public void ModelTest_ItemSourceChanged()
        {
            int propChanging = 0;
            int propChanged = 0;
            string propNameChanged = "";
            string propNameChanging = "";
            bool itemsChanging = false;
            bool itemsChanged = false;
            ControlDefinition.PropertyChanging += (o, e) =>
            {
                Assert.IsNotNull(e?.PropertyName);
                propChanging++;
                propNameChanging = e.PropertyName;
                itemsChanging |= e.PropertyName == nameof(ControlDefinition.Items);
            };
            ControlDefinition.PropertyChanged += (o, e) =>
            {
                Assert.IsNotNull(e?.PropertyName);
                propChanged++;
                propNameChanged = e.PropertyName;
                itemsChanged |= e.PropertyName == nameof(ControlDefinition.Items);
            };

            ControlDefinition.Items = SelectorTestItem.CreateArray();
            Assert.IsTrue(itemsChanged);
            Assert.IsTrue(itemsChanging);
        }

        [TestMethod]
        public void ModelTest_DisplaymemberPath()
        {
            int propChanging = 0;
            int propChanged = 0;
            string propNameChanged = "";
            string propNameChanging = "";
            ControlDefinition.PropertyChanging += (o, e) =>
            {
                Assert.IsNotNull(e?.PropertyName);
                propChanging++;
                propNameChanging = e.PropertyName;
            };
            ControlDefinition.PropertyChanged += (o, e) =>
            {
                Assert.IsNotNull(e?.PropertyName);
                propChanged++;
                propNameChanged = e.PropertyName;
            };

            ControlDefinition.DisplayMemberPath = "value";
            Assert.AreEqual(1, propChanging);
            Assert.AreEqual(1, propChanged);
            Assert.AreEqual(nameof(ControlDefinition.DisplayMemberPath), propNameChanging);
            Assert.AreEqual(nameof(ControlDefinition.DisplayMemberPath), propNameChanged);
        }

        [STATestMethod]
        public virtual void ControlTest_DisplayMemberPath()
        {
            var itemCntrl = GetItemsControl();
            Assert.IsNotNull(itemCntrl);
            Assert.AreEqual(ControlDefinition.DisplayMemberPath, itemCntrl.DisplayMemberPath, "DisplayMemberPath does not match the ControlDefinition");
        }

        [STATestMethod]
        public virtual void ControlTest_Items()
        {
            var itemCntrl = GetItemsControl();
            Assert.IsNotNull(itemCntrl);
            Assert.AreEqual(SelectorTestItem.CreateArray().Length, itemCntrl.Items.Count, "ItemsControl definition has incorrect number of items");
        }
    }
}