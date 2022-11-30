using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.WPFControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RFBCodeWorks.WPFControls.Tests
{
    [TestClass()]
    public class AutoStackGridTests
    {
        [TestMethod()]
        public void AutoStackGridTest()
        {
            var btn1 = new Button();
            var btn2 = new Button();
            var btn3 = new Button();
            var btn4 = new Button();
            var btn5 = new Button();

            var grid = new RFBCodeWorks.WPFControls.AutoWrapGrid();
            
            grid.ColumnCount = 4;

            grid.Children.Add(btn1);
            grid.Children.Add(btn2);
            grid.Children.Add(btn3);
            grid.Children.Add(btn4);
            grid.Children.Add(btn5);

            Assert.AreEqual(0, Grid.GetColumn(btn1));
            Assert.AreEqual(1, Grid.GetColumn(btn2));
            Assert.AreEqual(2, Grid.GetColumn(btn3));
            Assert.AreEqual(3, Grid.GetColumn(btn4));
            Assert.AreEqual(0, Grid.GetColumn(btn5));

            Assert.AreEqual(0, Grid.GetRow(btn1));
            Assert.AreEqual(0, Grid.GetRow(btn2));
            Assert.AreEqual(0, Grid.GetRow(btn3));
            Assert.AreEqual(0, Grid.GetRow(btn4));
            Assert.AreEqual(1, Grid.GetRow(btn5));

            //With padding, all items should be on interior 2 columns
            grid.Padding = "10";
            var btn6 = new Button();
            grid.Children.Add(btn6);

            Assert.AreEqual(1, Grid.GetColumn(btn1));
            Assert.AreEqual(2, Grid.GetColumn(btn2));
            Assert.AreEqual(1, Grid.GetColumn(btn3));
            Assert.AreEqual(2, Grid.GetColumn(btn4));
            Assert.AreEqual(1, Grid.GetColumn(btn5));
            Assert.AreEqual(2, Grid.GetColumn(btn6));

            Assert.AreEqual(1, Grid.GetRow(btn1));
            Assert.AreEqual(1, Grid.GetRow(btn2));
            Assert.AreEqual(2, Grid.GetRow(btn3));
            Assert.AreEqual(2, Grid.GetRow(btn4));
            Assert.AreEqual(3, Grid.GetRow(btn5));
            Assert.AreEqual(3, Grid.GetRow(btn6));

        }
    }
}