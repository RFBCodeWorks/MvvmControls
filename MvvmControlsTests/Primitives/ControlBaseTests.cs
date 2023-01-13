using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

namespace RFBCodeWorks.Mvvvm.Primitives.Tests
{
    [TestClass()]
    public class ControlBaseTests
    {
        public ControlBaseTests() :this(new()) { }
        /// <summary>
        /// Set the ControlDefinition for the test methods
        /// </summary>
        /// <param name="definition"></param>
        public ControlBaseTests(ControlBase definition)
        {
            ControlDefinition = definition;
        }

        protected ControlBase ControlDefinition { get; }

        [TestCleanup]
        public virtual void TestCleanup()
        {

        }

        [TestInitialize]
        public virtual void TestInitialize()
        {

        }

        [TestMethod]
        public void TestTooltip()
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
            ControlDefinition.ToolTip = "NewToolTip";
            Assert.AreEqual("NewToolTip", ControlDefinition.ToolTip);
            Assert.AreEqual(nameof(ControlDefinition.ToolTip), propNameChanging); propNameChanging = "";
            Assert.AreEqual(nameof(ControlDefinition.ToolTip), propNameChanged); propNameChanged = "";
            ControlDefinition.ToolTip = "NewToolTip2";
            Assert.AreEqual("NewToolTip2", ControlDefinition.ToolTip);
            Assert.AreEqual(nameof(ControlDefinition.ToolTip), propNameChanging); propNameChanging = "";
            Assert.AreEqual(nameof(ControlDefinition.ToolTip), propNameChanged); propNameChanged = "";
            Assert.AreEqual(2, propChanging);
            Assert.AreEqual(2, propChanged);
        }

        [TestMethod]
        public void TestEnabled()
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
            Assert.IsTrue(ControlDefinition.IsEnabled);
            ControlDefinition.IsEnabled = false;
            Assert.IsFalse(ControlDefinition.IsEnabled);
            Assert.AreEqual(nameof(ControlDefinition.IsEnabled), propNameChanging); propNameChanging = "";
            Assert.AreEqual(nameof(ControlDefinition.IsEnabled), propNameChanged); propNameChanged = "";
            ControlDefinition.IsEnabled = true;
            Assert.IsTrue(ControlDefinition.IsEnabled);
            Assert.AreEqual(nameof(ControlDefinition.IsEnabled), propNameChanging); propNameChanging = "";
            Assert.AreEqual(nameof(ControlDefinition.IsEnabled), propNameChanged); propNameChanged = "";
            Assert.AreEqual(2, propChanging);
            Assert.AreEqual(2, propChanged);
        }
        
        [TestMethod]
        public void TestVisibility()
        {

            int propChanging = 0;
            int propChanged = 0;
            bool visChanged = false;
            ControlDefinition.PropertyChanging += (o, e) => propChanging++;
            ControlDefinition.PropertyChanged += (o, e) => propChanged++;
            ControlDefinition.VisibilityChanged += (o, e) => visChanged = true;

            Assert.AreEqual(Visibility.Visible, ControlDefinition.Visibility);
            Assert.AreEqual(Visibility.Collapsed, ControlDefinition.HiddenMode);
            ControlDefinition.IsVisible = false;
            Assert.IsTrue(visChanged);
            Assert.AreEqual(ControlDefinition.HiddenMode, ControlDefinition.Visibility, "Visibility was not set to the HiddenMode value");
            Assert.IsFalse(ControlDefinition.IsVisible);
            Assert.AreEqual(2, propChanging, "PropertyChanging did not fire the expected amount of times");
            Assert.AreEqual(2, propChanged, "PropertyChanged did not fire the expected amount of times");


            ControlDefinition.HiddenMode = Visibility.Hidden;
            Assert.IsFalse(ControlDefinition.IsVisible);
            Assert.AreEqual(ControlDefinition.HiddenMode, ControlDefinition.Visibility, "Visibility was not set to the HiddenMode value");

            ControlDefinition.HiddenMode = Visibility.Collapsed;
            Assert.IsFalse(ControlDefinition.IsVisible);
            Assert.AreEqual(ControlDefinition.HiddenMode, ControlDefinition.Visibility, "Visibility was not set to the HiddenMode value");

            ControlDefinition.IsVisible = true;
            Assert.AreEqual(Visibility.Visible, ControlDefinition.Visibility);
        }

        protected virtual void TestControlInteraction(System.Windows.Controls.Control cntrl)
        {
            //Test Visibility
            ControlDefinition.IsVisible = true;
            Assert.AreEqual(Visibility.Visible, cntrl.Visibility, "Control did not become Visible when ViewModel set IsVisible");
            ControlDefinition.IsVisible = false;
            Assert.AreEqual(ControlDefinition.HiddenMode, cntrl.Visibility, "Control did not become Hidden when ViewModel set IsVisible");
            ControlDefinition.Visibility = Visibility.Visible;
            Assert.AreEqual(Visibility.Visible, cntrl.Visibility, "Control did not become Visible when ViewModel set IsVisible");

            //Test Enabled
            ControlDefinition.IsEnabled = true;
            Assert.IsTrue(cntrl.IsEnabled, "Control did not become enabled when ControlModel was updated");
            ControlDefinition.IsEnabled = false;
            Assert.IsFalse(cntrl.IsEnabled, "Control did not become disabled when ControlModel was updated");

        }
    }
}