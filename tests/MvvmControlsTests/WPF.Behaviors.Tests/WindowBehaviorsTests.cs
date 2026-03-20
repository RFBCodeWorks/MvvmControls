using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Tests;
using RFBCodeWorks.WPF.Behaviors;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.WPF.Behaviors.Tests
{
    [TestClass]
    public class WindowBehaviorsTests
    {
        [STATestMethod]
        [DataRow(data: true, DisplayName = "Subscribed")]
        [DataRow(data: false, DisplayName = "Unsubcribed")]
        public void IWindowActivatedHandlerTest(bool subscribe)
        {
            Window? w = new(), w2 = new();
            try
            {

                var handler = new WindowBehaviorTests2.WindowHandlerObj();
                WindowBehaviors.SetIWindowActivatedHandler(w, handler);
                Assert.AreSame(handler, WindowBehaviors.GetIWindowActivatedHandler(w));

                if (!subscribe)
                {
                    WindowBehaviors.SetIWindowActivatedHandler(w, null);
                    Assert.IsNull(WindowBehaviors.GetIWindowActivatedHandler(w));
                }

                w2.Show();
                w.Show();
                Assert.AreEqual(subscribe, handler.WasActivated, " -- 'WasActivated' check failed.");
                Assert.IsFalse(handler.WasDeactivated, " -- 'WasDeactivated' was true unexpectedly.");

                w2.Activate();
                Assert.AreEqual(subscribe, handler.WasDeactivated, subscribe ? " -- 'WasDeactivated' was not set after window was deactivated" : "");
            }
            finally
            {
                WindowBehaviors.SetIWindowActivatedHandler(w, null);
                w.Close();
                w2.Close();
            }
        }

        [STATestMethod]
        [DataRow(data: true, DisplayName = "Subscribed")]
        [DataRow(data: false, DisplayName = "Unsubcribed")]
        public void IWindowClosingHandlerTest(bool subscribe)
        {
            Window w = new();
            try
            {

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
            finally
            {
                try {
                    WindowBehaviors.SetIWindowClosingHandler(w, null); 
                    w.Close();
                } catch { }
            }
        }

        [STATestMethod]
        [DataRow(data: true, DisplayName = "Subscribed")]
        [DataRow(data: false, DisplayName = "Unsubcribed")]
        public void IWindowLoadingHandlerTest(bool subscribe)
        {
            Window w = new();
            try
            {
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
            }
            finally
            {
                WindowBehaviors.SetIWindowLoadingHandler(w, null);
                w.Close();
            }
        }

    }
}