using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RFBCodeWorks.WPF.Controls.Tests
{
    [TestClass()]
    public class NumericUpDownTests
    {
        [TestMethod()]
        public void IntegerUpDown()
        {
            var cntrl = new IntegerUpDown();
            Assert.IsNotNull(cntrl);
            Assert.AreEqual(0, cntrl.Value);
            cntrl.Value = 10;
            Assert.AreEqual(10, cntrl.Value);
            cntrl.Minimum = -50;
            cntrl.Value = -100;
            Assert.AreEqual(cntrl.Minimum, cntrl.Value);
            cntrl.Value = 500;
            Assert.AreNotEqual(500, cntrl.Value);
            Assert.AreEqual(cntrl.Maximum, cntrl.Value);
            cntrl.AllowOutsideRange = true;
            cntrl.Value = 500;
            Assert.AreEqual(500, cntrl.Value);
        }

        [TestMethod()]
        public void DecimalUpDown()
        {
            var cntrl = new DecimalUpDown();
            Assert.IsNotNull(cntrl);
            Assert.AreEqual(0, cntrl.Value);
            cntrl.Value = 10.525437;
            Assert.AreEqual(10.525437, cntrl.Value);
            cntrl.Minimum = -50;
            cntrl.Value = -100;
            Assert.AreEqual(cntrl.Minimum, cntrl.Value);
            cntrl.Value = 500;
            Assert.AreNotEqual(500, cntrl.Value);
            Assert.AreEqual(cntrl.Maximum, cntrl.Value);
            cntrl.AllowOutsideRange = true;
            cntrl.Value = 500;
            Assert.AreEqual(500, cntrl.Value);
        }
    }
}