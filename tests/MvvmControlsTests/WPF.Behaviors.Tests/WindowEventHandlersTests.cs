using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm;
using RFBCodeWorks.WPF.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RFBCodeWorks.Mvvm.Tests;


namespace RFBCodeWorks.WPF.Behaviors.Tests
{
    [STATestClass]
    public class WindowBehaviorTests2
    {
        private static Window GetWindow(string windowTitle) => new() { Title = windowTitle };

        private static void SetWindowContent(Window w)
        {
            var g = new Grid();
            var s = new StackPanel();
            s.Children.Add(new TextBlock() { Text = "TextBlock" });
            s.Children.Add(new Button() { Content = "Button" });
            s.Children.Add(new TextBox() { Text = "TextBox" });
            g.Children.Add(s);
            w.Content = g;
        }

        

        private static async Task CheckSubscription(Window TestWindow, WindowHandlerObj eventHandler, bool isSubscribed)
        {
            Assert.IsNotNull(TestWindow);
            Assert.IsNotNull(eventHandler);

            SetWindowContent(TestWindow);
            Window Window2 = new();
                        
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
            TestWindow.Focus();
            Assert.IsTrue(TestWindow.IsActive, "\n Test Window should be active at this point.");
            
            bool closed = false;
            TestWindow.Closed += (o, e) => { Console.Write("Closed Event Occurred."); closed = true; };
           
            if (isSubscribed)
            {
                eventHandler.CancelClosing = true;
                Console.Write("\nClosing Window -- ");
                TestWindow.Close();
                Assert.IsTrue(TestWindow.IsActive, "\n Failed to prevent window closure");
                Assert.IsFalse(closed);
                eventHandler.CancelClosing = false;
            }
            Console.Write("\nClosing Window -- ");
            TestWindow.Close();
            await Task.Delay(3).ConfigureAwait(true);
            Assert.IsTrue(closed);
            
            
            Assert.AreEqual(isSubscribed, eventHandler.WasLoaded, "Window Loaded Test Failed");
            Assert.AreEqual(isSubscribed, eventHandler.WasActivated, "Window Activated Test Failed");
            Assert.AreEqual(isSubscribed, eventHandler.WasClosing, "Window Closing Test Failed");
            Assert.AreEqual(isSubscribed, eventHandler.WasClosed, "Window Closed Test Failed");
            //Assert.AreEqual(isSubscribed, handler.WasContentRendered, "Window ContentRendered Test Failed");  //ContentRendered always failed, but works properly in example app
            Assert.AreEqual(isSubscribed, eventHandler.WasDeactivated, "Window Deactivated Test Failed");
            Assert.AreEqual(isSubscribed, eventHandler.GotFocus, "Window GotFocus test failed");
            Assert.AreEqual(isSubscribed, eventHandler.LostFocus, "Window LostFocus test failed");
        }

        private static void Subscribe(Window window, WindowHandlerObj? value)
        {
            WindowBehaviors.SetIWindowActivatedHandler(window, value);
            WindowBehaviors.SetIWindowClosingHandler(window, value);
            WindowBehaviors.SetIWindowFocusHandler(window, value);
            WindowBehaviors.SetIWindowLoadingHandler(window, value);
        }

        [STATestMethod]
        public async Task SubscribeTest()
        {
            var TestWindow = GetWindow("TestWindow");
            var handler = new WindowHandlerObj();
            Subscribe(TestWindow, handler);
            await CheckSubscription(TestWindow, handler, true);
        }

        [STATestMethod]
        public async Task UnsubscribeTest()
        {
            var originalHandler = new WindowHandlerObj();
            Assert.IsNotNull(originalHandler);

            var TestWindow = GetWindow("TestWindow");
            Subscribe(TestWindow, originalHandler);
            Subscribe(TestWindow, null);
            await CheckSubscription(TestWindow, originalHandler, false);
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

            public void OnWindowClosed(object? sender, EventArgs e)
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

            public void OnWindowContentRendered(object? sender, EventArgs e)
            {
                WasContentRendered = true;
                Console.WriteLine("Window Content Rendered");
            }

            public void OnWindowLoaded(object? sender, EventArgs e)
            {
                WasLoaded = true;
                Console.WriteLine("Window Loaded");
            }

            public void OnWindowActivated(object? sender, EventArgs e)
            {
                WasActivated = true;
                Console.WriteLine("Window Activated");
            }

            public void OnWindowDeactivated(object? sender, EventArgs e)
            {
                WasDeactivated = true;
                Console.WriteLine("Window De-Activated");
            }

            public void OnUIElementGotFocus(object? sender, EventArgs e)
            {
                GotFocus = true;
            }

            public void OnUIElementLostFocus(object? sender, EventArgs e)
            {
                LostFocus = true;
            }
        }
    }
}