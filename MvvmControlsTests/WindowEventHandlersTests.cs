using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.MvvmControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.MvvmControls.Tests
{
    [TestClass()]
    public class WindowEventHandlersTests
    {
        public WindowEventHandlersTests()
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
            TestWindow.Activate();
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
            //Assert.AreEqual(isSubscribed, handler.WasContentRendered, "Window ContentRendered Test Failed");
            Assert.AreEqual(isSubscribed, handler.WasDeactivated, "Window Deactivated Test Failed");
            WindowEventHandlers.Unsubscribe(TestWindow);
        }

        [TestMethod()]
        public void SubscribeTest()
        {
            var TestWindow = GetWindow("TestWindow");
            WindowEventHandlers.Subscribe(TestWindow);
            CheckSubscription(TestWindow, new(), true);
        }

        [TestMethod()]
        public void UnsubscribeTest()
        {
            var TestWindow = GetWindow("TestWindow");
            WindowEventHandlers.Subscribe(TestWindow);
            WindowEventHandlers.Unsubscribe(TestWindow);
            CheckSubscription(TestWindow, new(), false);
        }

        internal class WindowHandlerObj : IWindowActivated, IWindowClosing, IWindowLoading
        {
            public bool WasLoaded;
            public bool WasClosed;
            public bool WasClosing;
            public bool WasContentRendered;
            public bool WasActivated;
            public bool WasDeactivated;

            public bool CancelClosing;

            public void OnClosed()
            {
                WasClosed = true;
                Console.WriteLine("Window Closed");
            }

            public bool OnClosing()
            {
                WasClosing = true;
                if (CancelClosing)
                    Console.WriteLine("Window Closing Cancelled"); 
                else
                    Console.WriteLine("Window Closing");
                return !CancelClosing;
            }

            public void OnContentRendered()
            {
                WasContentRendered = true;
                Console.WriteLine("Window Content Rendered");
            }

            public void OnLoaded()
            {
                WasLoaded = true;
                Console.WriteLine("Window Loaded");
            }

            public void OnWindowActivated()
            {
                WasActivated = true;
                Console.WriteLine("Window Activated");
            }

            public void OnWindowDeactivated()
            {
                WasDeactivated = true;
                Console.WriteLine("Window De-Activated");
            }
        }
    }
}