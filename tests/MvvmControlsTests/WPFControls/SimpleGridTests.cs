using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.WPF.Controls.Tests
{
    [TestClass()]
    public class SimpleGridTests
    {


        [TestMethod()]
        public void CreateGrid()
        {
            var g = new SimpleGrid();
            Assert.AreEqual(0, g.RowCount);
            Assert.AreEqual(0, g.ColumnCount);

            g.RowCount = 5;
            Assert.AreEqual(5, g.RowCount, "\n\n RowCount does not match expected count!");
            g.ColumnCount = 5;            
            Assert.AreEqual(5, g.ColumnCount, "\n\n Column does not match expected count!");

            g.RowCount = 9;
            Assert.AreEqual(9, g.RowCount, "\n\n RowCount does not match expected count!");
            g.ColumnCount = 7;
            Assert.AreEqual(7, g.ColumnCount, "\n\n Column does not match expected count!");

            g.RowCount = 3;
            Assert.AreEqual(3, g.RowCount, "\n\n RowCount does not match expected count!");
            g.ColumnCount = 4;
            Assert.AreEqual(4, g.ColumnCount, "\n\n Column does not match expected count!");
        }

        [TestMethod()]
        public void AssignPadding()
        {
            var g = new SimpleGrid
            {
                AutoColumnWidth = new(24),
                AutoRowHeight = new(38),

                Padding = "5",
                ColumnCount = 5,
                RowCount = 5
            };
            Assert.AreEqual(5, g.ColumnDefinitions[0].Width.Value, "\n\n Width was not set for first column!");
            Assert.AreEqual(5, g.RowDefinitions[0].Height.Value, "\n\n Height was not set for first row!");
            Assert.AreEqual(5, g.ColumnDefinitions[4].Width.Value, "\n\n Width was not set for last column!");
            Assert.AreEqual(5, g.RowDefinitions[4].Height.Value, "\n\n Height was not set for last row!");
            
            Assert.AreEqual(g.AutoColumnWidth, g.ColumnDefinitions[2].Width, "\n\n Width for interior column does not match AutoHeight");
            Assert.AreEqual(g.AutoRowHeight, g.RowDefinitions[2].Height, "\n\n Width for interior column does not match AutoHeight");

            g.Padding = "10,7";
            Assert.AreEqual(10, g.ColumnDefinitions[0].Width.Value, "\n\n Width was not set for first column!");
            Assert.AreEqual(7, g.RowDefinitions[0].Height.Value, "\n\n Height was not set for first row!");
            Assert.AreEqual(10, g.ColumnDefinitions[4].Width.Value, "\n\n Width was not set for last column!");
            Assert.AreEqual(7, g.RowDefinitions[4].Height.Value, "\n\n Height was not set for last row!");

            g.Padding = null;
            Assert.AreEqual(g.AutoColumnWidth, g.ColumnDefinitions[0].Width, "\n\n Width was not set for first column!");
            Assert.AreEqual(g.AutoRowHeight, g.RowDefinitions[0].Height, "\n\n Height was not set for first row!");
            Assert.AreEqual(g.AutoColumnWidth, g.ColumnDefinitions[4].Width, "\n\n Width was not set for last column!");
            Assert.AreEqual(g.AutoRowHeight, g.RowDefinitions[4].Height, "\n\n Height was not set for last row!");

            g.Padding = "1,2,3,4";
            Assert.AreEqual(1, g.ColumnDefinitions[0].Width.Value, "\n\n Width was not set for first column!");
            Assert.AreEqual(2, g.RowDefinitions[0].Height.Value, "\n\n Height was not set for first row!");
            Assert.AreEqual(3, g.ColumnDefinitions[4].Width.Value, "\n\n Width was not set for last column!");
            Assert.AreEqual(4, g.RowDefinitions[4].Height.Value, "\n\n Height was not set for last row!");

            g.Padding = "";
            Assert.AreEqual(g.AutoColumnWidth, g.ColumnDefinitions[0].Width, "\n\n Width was not set for first column!");
            Assert.AreEqual(g.AutoRowHeight, g.RowDefinitions[0].Height, "\n\n Height was not set for first row!");
            Assert.AreEqual(g.AutoColumnWidth, g.ColumnDefinitions[4].Width, "\n\n Width was not set for last column!");
            Assert.AreEqual(g.AutoRowHeight, g.RowDefinitions[4].Height, "\n\n Height was not set for last row!");

            Assert.ThrowsException<ArgumentException>(() => g.Padding = "INVALID");
            
        }
    }
}