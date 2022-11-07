using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.MVVMObjects.Tests
{
    [TestClass()]
    public class ItemSourceDefinitionTests : BaseControlDefinitionTests
    {
        public ItemSourceDefinitionTests() :this(new()) { }

        /// <summary>
        /// Set the ItemSourceDefinitionTests for the test methods
        /// </summary>
        /// <param name="definition"></param>
        public ItemSourceDefinitionTests(ItemSourceDefinition<SelectorTestItem, SelectorTestItem[]> definition) : base(definition)
        {
            ControlDefinition = definition;
        }

        new protected ItemSourceDefinition<SelectorTestItem, SelectorTestItem[]> ControlDefinition { get; }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            ControlDefinition.ItemSource = SelectorTestItem.CreateArray();
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

            ControlDefinition.ItemSource = SelectorTestItem.CreateArray();
            Assert.AreEqual(1, propChanging);
            Assert.AreEqual(1, propChanged);
            Assert.AreEqual(nameof(ControlDefinition.ItemSource), propNameChanging);
            Assert.AreEqual(nameof(ControlDefinition.ItemSource), propNameChanged);
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

    public record SelectorTestItem(int Value, string Name)
    {
        public static SelectorTestItem Zero { get; } = new(0, "Zero");
        public static SelectorTestItem One { get; } = new(1, "One");
        public static SelectorTestItem Two { get; } = new(2, "Two");
        public static SelectorTestItem Three { get; } = new(3, "Three");
        public static SelectorTestItem Four { get; } = new(4, "Four");
        public static SelectorTestItem Five { get; } = new(5, "Five");
        public static SelectorTestItem Six { get; } = new(6, "Six");
        public static SelectorTestItem Seven { get; } = new(7, "Seven");
        public static SelectorTestItem Eight { get; } = new(8, "Eight");
        public static SelectorTestItem Nine { get; } = new(9, "Nine");

        public static implicit operator SelectorTestItem(int value) => CreateArray().Single(i => i.Value == value);
        public static explicit operator int(SelectorTestItem value) => value.Value;

        public static SelectorTestItem[] CreateArray()
        {
            return new SelectorTestItem[] { Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine };
        }
    }
}