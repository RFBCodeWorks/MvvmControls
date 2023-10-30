using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Primitives.Tests
{
    [TestClass()]
    public class TreeViewItemBaseTests
    {
        [TestMethod()]
        public void DeselectedEventTest()
        {
            bool selectRaised = false;
            bool childSelectRaised = false;
            bool deselectRaised = false;
            var parent = new TreeItem();
            parent.IsSelected = true;
            parent.Selected += AcknowledgeSelect;
            parent.Deselected += AcknowledgeDeselect;
            parent.ChildSelected += AcknowledgeChildSelect;
            parent.IsSelected = false;
            Assert.IsTrue(deselectRaised, "\n\nEvent was not raised");
            Assert.IsFalse(selectRaised, "\n\nSelected was raised incorrectly");
            Assert.IsFalse(childSelectRaised, "\n\nChildSelected was raised incorrectly");

            void AcknowledgeSelect(object sender, EventArgs e) => selectRaised = true;
            void AcknowledgeDeselect(object sender, EventArgs e) => deselectRaised = true;
            void AcknowledgeChildSelect(object sender, EventArgs e) => childSelectRaised = true;
        }

        [TestMethod()]
        public void SelectedEventTest()
        {
            bool selectRaised = false;
            bool childSelectRaised = false;
            bool deselectRaised = false;
            var parent = new TreeItem();
            parent.IsSelected = false;
            parent.Selected += AcknowledgeSelect;
            parent.Deselected += AcknowledgeDeselect;
            parent.ChildSelected += AcknowledgeChildSelect;
            parent.IsSelected = true;
            Assert.IsTrue(selectRaised, "\n\nEvent was not raised");
            Assert.IsFalse(deselectRaised, "\n\nDeselected was raised incorrectly");
            Assert.IsFalse(childSelectRaised, "\n\nChildSelected was raised incorrectly");

            void AcknowledgeSelect(object sender, EventArgs e) => selectRaised = true;
            void AcknowledgeDeselect(object sender, EventArgs e) => deselectRaised= true;
            void AcknowledgeChildSelect(object sender, EventArgs e) => childSelectRaised = true;
        }

        [TestMethod()]
        public void ChildSelectedEventBubbleUpTest()
        {
            bool eventRaised = false;
            var parent = new TreeItem();
            parent.ChildSelected += Acknowledge;
            var child1 = new TreeItem();
            var child2 = new TreeItem();
            parent.AddItem(child1);
            child1.AddItem(child2);

            child1.IsSelected = true;
            Assert.IsTrue(eventRaised, "Child 1 Event was not raised");
            child1.IsSelected = false;
            eventRaised = false;
            child2.IsSelected = true;
            Assert.IsTrue(eventRaised, "Child 2 Event was not raised");

            void Acknowledge(object sender, EventArgs e) => eventRaised = true;
        }

        [TestMethod()]
        public void ChildSelectedEventBubbleUpTest2()
        {
            // Test the DIrectory Child Selected
            bool eventRaised = false;
            var parent = new Specialized.TreeViewDirectoryInfo(new System.IO.DirectoryInfo(Environment.CurrentDirectory));
            parent.ChildSelected += Acknowledge;
            var firstItem = parent.Children.First();
            firstItem.IsSelected = true;

            Assert.IsTrue(eventRaised, "Child 1 Event was not raised");
            void Acknowledge(object sender, EventArgs e) => eventRaised = true;
        }

        private class TreeItem : TreeViewItemBase
        {
            private List<TreeItem> Children { get; } = new List<TreeItem>();
            protected override IEnumerable<ITreeViewItem> ITreeViewChildren => Children;
            
            public void AddItem(TreeItem t)
            {
                Children.Add(t);
                SubscribeToChild(t);
            }

            public void RemoveItem(TreeItem t)
            {
                Children.Remove(t);
                UnsubscribeFromChild(t);
            }
        }
    }
}