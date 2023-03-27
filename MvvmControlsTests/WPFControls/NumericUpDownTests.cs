using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

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
            Assert.AreEqual(betweenMinMax, cntrl.Value, "\n-- Increased values above the Maximum when IncreaseValueCommand was called!");

            Assert.IsTrue(cntrl.DecreaseValueCommand.CanExecute(null), "DecreaseValueCommand return CanExecute=false");
        }

        /// <summary>
        /// Tests for Issue #4 - <see href="https://github.com/RFBCodeWorks/MvvmControls/issues/4"/>
        /// </summary>
        private void TestIssue4<T, I>(T cntrl, I zero, I incrementValue)
            where I : struct, IComparable<I>, IEquatable<I>, IFormattable
            where T : Control, RFBCodeWorks.WPF.Controls.Primitives.IUpDown 
        {
            Assert.AreNotEqual(zero, incrementValue, "Test not set up properly - IncrementValue should be greater than 0");
            //Assert.AreEqual(incrementValue, max, "Test not set up properly - Max should be equal to Increment Value");

            var xelement = new System.Xml.Linq.XElement("ValueNode");
            var obj = new RFBCodeWorks.Mvvm.XmlLinq.ValueSetters.XIntegerSetter(new RFBCodeWorks.Mvvm.XmlLinq.XElementWrapper(xelement));
            var prop = RFBCodeWorks.WPF.Controls.Primitives.UpDownBase<I>.ValueProperty;
            var iface = cntrl as Primitives.IUpDown;
            // Bind the VALUE property
            var bind = new Binding(nameof(obj.Value))
            {
                Source = obj,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            var bindexpr = BindingOperations.SetBinding(cntrl, prop, bind);

            
            cntrl.Minimum = zero;
            cntrl.Maximum = incrementValue;
            cntrl.Value = zero;
            cntrl.SmallChange = incrementValue;
            cntrl.LargeChange = incrementValue;

            Assert.AreEqual(0, obj.Value, "Bound Object value not set to zero");
            
            //Test increase
            iface.IncreaseValueCommand.Execute(null);
            Assert.AreEqual(incrementValue, iface.Value, "\n\n--Control value was not incremented");          
            Assert.AreEqual(Convert.ToInt32(iface.Value), obj.Value, "\n\n--Bound Object value was not set to incremented value");
            Console.WriteLine("- Stage 1 - Successfully incremented bound value");

            //Test increase past max - should not increase
            iface.IncreaseValueCommand.Execute(null);
            Assert.AreEqual(incrementValue, iface.Value, "\n\n--Control incremented when it should not have been (was already at max)");
            Assert.AreEqual(Convert.ToInt32(iface.Value), obj.Value, "\n\n--Bound Object does not match the control's value");
            Console.WriteLine("- Stage 2 - Increment past maximum test succeeded - bound value and control value are still identical");

            //Reset to 0 -- Test Decrease
            iface.DecreaseValueCommand.Execute(null);
            System.Threading.Thread.Sleep(5);
            Assert.AreEqual(zero, iface.Value, "\n\n--Control did not decrement value");
            Assert.AreEqual(Convert.ToInt32(iface.Value), obj.Value, "\n\n--Bound Object does not match the control's value");
            Console.WriteLine("- Stage 3 - Successfully decremented bound value");

            //Test Decrease less than zero
            iface.DecreaseValueCommand.Execute(null);
            Assert.AreEqual(zero, iface.Value, "\n\n--Control decremented when it should not have been (was already at min)");
            Assert.AreEqual(Convert.ToInt32(iface.Value), obj.Value, "\n\n--Bound Object does not match the control's value");
            Console.WriteLine("- Stage 4 - Increment below minimum test succeeded - bound value and control value are still identical");
        }



        [TestMethod()]
        public void IntegerUpDown()
        {
            var cntrl = new IntegerUpDown();
            TestIssue4(cntrl, 0, 5);
            TestUpDown(cntrl, 0, -100, 100, 25, -500, 500);
        }

        [TestMethod()]
        public void DecimalUpDown()
        {
            var cntrl = new DecimalUpDown();
            TestIssue4(cntrl, (double)0, 5);
            TestUpDown(cntrl, (double)0, -100, 100, 25, -500, 500);
        }
    }
}