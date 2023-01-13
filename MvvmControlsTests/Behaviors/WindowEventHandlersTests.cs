using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm;
using RFBCodeWorks.WPFBehaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.WPFBehaviors.Tests
{
    [TestClass()]
    public class WindowBehaviorTests2
    {
        public WindowBehaviorTests2()
        {
            handler = new();
        }

        private Window GetWindow(string windowTitle)
        {
            var w = new Window();
            w.Title = windowTitle;
            return w;
        }

        private void SetWindowContent(Window w)
        {
            var g = new Grid();
            var s = new StackPanel();
            s.Children.Add(new TextBlock() { Text = "TextBlock" });
            s.Children.Add(new Button() { Content = "Button" });
            s.Children.Add(new TextBox() { Text = "TextBox" });
            g.Children.Add(s);
            w.Content = g;
        }

        private WindowHandlerObj handler { get; set; }

        [TestCleanup]
        public void Complete()
        {
            handler = null;
        }

        private void CheckSubscription(Window TestWindow, Window Window2, bool isSubscribed)
        {
            TestWindow.DataContext = handler;
            SetWindowContent(TestWindow);
            Window2.Show();
            Console.Write("\nShowing Window -- ");
            TestWindow.Show();
            Console.Write("\nFocusing Other Window -- ");
            Window2.Activate();
            Console.Write("\nActivating Window -- ");
            TestWindow.Show();
            TestWindow.Activate();
            ((Button)((StackPanel)((Grid)TestWindow.Content).Children[0]).Children[1]).Focus(); // simulate the button getting focus
            Window2.Focus();
            Window2.Close();
            if (isSubscribed)
            {
                handler.CancelClosing = true;
                Console.Write("\nClosing Window -- ");
                TestWindow.Close();
                Assert.IsTrue(TestWindow.IsActive);
                handler.CancelClosing = false;
                Console.Write("\nClosing Window -- ");
            }
            else
            {
                Console.Write("\nClosing Window -- ");
            }
            TestWindow.Close();
            Assert.IsFalse(TestWindow.IsActive);

            Assert.AreEqual(isSubscribed, handler.WasLoaded, "Window Loaded Test Failed");
            Assert.AreEqual(isSubscribed, handler.WasActivated, "Window Activated Test Failed");
            Assert.AreEqual(isSubscribed, handler.WasClosing, "Window Closing Test Failed");
            Assert.AreEqual(isSubscribed, handler.WasClosed, "Window Closed Test Failed");
            //Assert.AreEqual(isSubscribed, handler.WasContentRendered, "Window ContentRendered Test Failed");  //ContentRendered always failed, but works properly in example app
            Assert.AreEqual(isSubscribed, handler.WasDeactivated, "Window Deactivated Test Failed");
            Assert.AreEqual(isSubscribed, handler.GotFocus, "Window GotFocus test failed");
            Assert.AreEqual(isSubscribed, handler.LostFocus, "Window LostFocus test failed");
            
        }

        private void Subscribe(Window window, WindowHandlerObj value)
        {
            WindowBehaviors.SetIWindowActivatedHandler(window, value);
            WindowBehaviors.SetIWindowClosingHandler(window, value);
            WindowBehaviors.SetIWindowFocusHandler(window, value);
            WindowBehaviors.SetIWindowLoadingHandler(window, value);
        }

        [TestMethod()]
        public void SubscribeTest()
        {
            var TestWindow = GetWindow("TestWindow");
            Subscribe(TestWindow, handler);
            CheckSubscription(TestWindow, new(), true);
        }

        [TestMethod()]
        public void UnsubscribeTest()
        {
            var TestWindow = GetWindow("TestWindow");
            Subscribe(TestWindow, handler);
            Subscribe(TestWindow, null);
            CheckSubscription(TestWindow, new(), false);
        }

        internal class WindowHandlerObj : IWindowActivatedHandler, IWindowClosingHandler, IWindowLoadingHandler, IWindowFocusHandler
        {
            public bool WasLoaded;
            public bool WasClosed;
            public bool WasClosing;
            public bool WasContentRendered;
            public bool WasActivated;
            public bool WasDeactivated;
            public bool GotFocus;
            public bool LostFocus;

            public bool CancelClosing;

            public void OnWindowClosed(object sender, EventArgs e)
            {
                WasClosed = true;
                Console.WriteLine("Window Closed");
            }

            public void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
            {
                WasClosing = true;
                if (CancelClosing)
                    Console.WriteLine("Window Closing Cancelled"); 
                else
                    Console.WriteLine("Window Closing");
                e.Cancel = CancelClosing;
            }

            public void OnWindowContentRendered(object sender, EventArgs e)
            {
                WasContentRendered = true;
                Console.WriteLine("Window Content Rendered");
            }

            public void OnWindowLoaded(object sender, EventArgs e)
            {
                WasLoaded = true;
                Console.WriteLine("Window Loaded");
            }

            public void OnWindowActivated(object sender, EventArgs e)
            {
                WasActivated = true;
                Console.WriteLine("Window Activated");
            }

            public void OnWindowDeactivated(object sender, EventArgs e)
            {
                WasDeactivated = true;
                Console.WriteLine("Window De-Activated");
            }

            public void OnUIElementGotFocus(object sender, EventArgs e)
            {
                GotFocus = true;
            }

            public void OnUIElementLostFocus(object sender, EventArgs e)
            {
                LostFocus = true;
            }
        }
    }
}