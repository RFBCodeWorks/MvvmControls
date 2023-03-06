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
        private void TestUpDown<T, I>(T cntrl, I zero, I min, I max, I betweenMinMax, I belowMin, I aboveMax)
            where I : struct, IComparable<I>, IEquatable<I>, IFormattable
            where T: RFBCodeWorks.WPF.Controls.Primitives.UpDownBase<I>
            
        {

            Assert.IsNotNull(cntrl);
            Assert.AreEqual(zero, cntrl.Value);
            
            cntrl.Minimum = min;
            Assert.AreEqual(min, cntrl.Minimum, "\n-- Failed to set Minimum value");

            cntrl.Maximum = max;
            Assert.AreEqual(max, cntrl.Maximum, "\n-- Failed to set Maximum value");

            cntrl.Value = betweenMinMax;
            Assert.AreEqual(betweenMinMax, cntrl.Value, "\n-- Failed to set VALUE between min and max");

            // Must stay within range
            cntrl.AllowOutsideRange = false;
            cntrl.Value = belowMin;
            Assert.AreNotEqual(belowMin, cntrl.Value, "\n-- Set value BELOW minimum when that should not be possible");
            Assert.AreEqual(cntrl.Minimum, cntrl.Value, "\n-- Control was not set to the minimum value!");
            
            cntrl.Value = aboveMax;
            Assert.AreNotEqual(aboveMax, cntrl.Value, "\n-- Set value ABOVE maximum when that should not be possible");
            Assert.AreEqual(cntrl.Maximum, cntrl.Value, "\n-- Control was not set to the maximum value!");

            //Allow outside range
            cntrl.AllowOutsideRange = true;
            cntrl.Value = belowMin;
            Assert.AreEqual(belowMin, cntrl.Value, "\n-- Value was not set! (Below minimum test)");
            cntrl.Value = aboveMax;
            Assert.AreEqual(aboveMax, cntrl.Value, "\n-- Value was not set! (Above maximum test)");


            //Test Increment/Decrement
            cntrl.AllowOutsideRange = false;
            cntrl.Value = zero;
            cntrl.SmallChange = betweenMinMax;
            cntrl.LargeChange = betweenMinMax;
            cntrl.Minimum = zero;
            cntrl.Maximum = betweenMinMax;

            cntrl.Value = zero;
            Assert.AreEqual(zero, cntrl.Value);
            
            Assert.IsFalse(cntrl.DecreaseValueCommand.CanExecute(null), "\n-- DecreaseValueCommand return CanExecute=true");
            Assert.IsTrue(cntrl.IncreaseValueCommand.CanExecute(null), "\n-- DecreaseValueCommand return CanExecute=false");
            cntrl.IncreaseValueCommand.Execute(betweenMinMax); // SHould increase value by smallChange
            Assert.AreEqual(betweenMinMax, cntrl.Value, "\n-- Failed to increase the value when IncreaseValueCommand was called!");

            Assert.IsTrue(cntrl.DecreaseValueCommand.CanExecute(null), "\n-- DecreaseValueCommand return CanExecute=false");
            Assert.IsFalse(cntrl.IncreaseValueCommand.CanExecute(null), "\n-- DecreaseValueCommand return CanExecute=true");
            cntrl.IncreaseValueCommand.Execute(betweenMinMax); // Should attempt to increase the value, but fail
            Assert.AreNotEqual(betweenMinMax, cntrl.Value, "\n-- Increased values above the Maximum when IncreaseValueCommand was called!");

            Assert.IsTrue(cntrl.DecreaseValueCommand.CanExecute(null), "DecreaseValueCommand return CanExecute=false");
        }

        [TestMethod()]
        public void IntegerUpDown()
        {
            var cntrl = new IntegerUpDown();
            TestUpDown(cntrl, 0, -100, 100, 25, -500, 500);
        }

        [TestMethod()]
        public void DecimalUpDown()
        {
            var cntrl = new DecimalUpDown();
            TestUpDown(cntrl, (double)0, -100, 100, 25, -500, 500);
        }
    }
}