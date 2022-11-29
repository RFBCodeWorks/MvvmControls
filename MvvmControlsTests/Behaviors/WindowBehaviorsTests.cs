using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.WPFBehaviors;
using RFBCodeWorks.MvvmControls.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.WPFBehaviors.Tests
{
    [TestClass()]
    public class WindowBehaviorsTests
    {
        [TestMethod()]
        [DataRow(data1: true, DisplayName = "Subscribed")]
        [DataRow(data1: false, DisplayName = "Unsubcribed")]
        public void IWindowActivatedHandlerTest(bool subscribe)
        {
            Window w = new();
            var handler = new WindowBehaviorTests2.WindowHandlerObj();
            WindowBehaviors.SetIWindowActivatedHandler(w, handler);
            Assert.AreSame(handler, WindowBehaviors.GetIWindowActivatedHandler(w));

            if (!subscribe)
            {
                WindowBehaviors.SetIWindowActivatedHandler(w, null);
                Assert.IsNull(WindowBehaviors.GetIWindowActivatedHandler(w));
            }

            w.Show();
            Assert.AreEqual(subscribe, handler.WasActivated);
            Assert.IsFalse(handler.WasDeactivated);
            w.Activate();
            w.Close();
            Assert.AreEqual(subscribe, handler.WasDeactivated);
        }

        [TestMethod()]
        [DataRow(data1: true, DisplayName ="Subscribed")]
        [DataRow(data1: false, DisplayName = "Unsubcribed")]
        public void IWindowClosingHandlerTest(bool subscribe)
        {
            Window w = new();
            var handler = new WindowBehaviorTests2.WindowHandlerObj();
            WindowBehaviors.SetIWindowClosingHandler(w, handler);
            Assert.AreSame(handler, WindowBehaviors.GetIWindowClosingHandler(w));
            if (!subscribe)
            {
                WindowBehaviors.SetIWindowClosingHandler(w, null);
                Assert.IsNull(WindowBehaviors.GetIWindowClosingHandler(w));
            }

            w.Show();
            
            handler.CancelClosing = true;
            w.Close();
            Assert.AreEqual(subscribe, handler.WasClosing); handler.WasClosing = false;
            Assert.IsFalse(handler.WasClosed);
            
            handler.CancelClosing = false;
            w?.Close();
            Assert.AreEqual(subscribe, handler.WasClosing); 
            Assert.AreEqual(subscribe, handler.WasClosed);
        }

        [TestMethod()]
        [DataRow(data1: true, DisplayName = "Subscribed")]
        [DataRow(data1: false, DisplayName = "Unsubcribed")]
        public void IWindowLoadingHandlerTest(bool subscribe)
        {
            Window w = new();
            var handler = new WindowBehaviorTests2.WindowHandlerObj();
            WindowBehaviors.SetIWindowLoadingHandler(w, handler);
            Assert.AreSame(handler, WindowBehaviors.GetIWindowLoadingHandler(w));

            if (!subscribe)
            {
                WindowBehaviors.SetIWindowLoadingHandler(w, null);
                Assert.IsNull(WindowBehaviors.GetIWindowLoadingHandler(w));
            }

            w.Show();
            Assert.AreEqual(subscribe, handler.WasLoaded);
            w.Close();
        }

    }
}