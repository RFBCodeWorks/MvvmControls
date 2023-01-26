using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Primitives;
using RFBCodeWorks.Mvvm.Tests;
using RFBCodeWorks.Mvvm.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace RFBCodeWorks.Mvvm.Primitives.Tests
{
    [TestClass()]
    public class CommandBaseTests
    {

        private class ConcreteCommand : CommandBase 
        {
            public ConcreteCommand() : base(false)
            {
            }
        }

        protected virtual CommandBase GetCommand() => new ConcreteCommand();


        private void CommandManagerTest()
        {
            bool requerySuggested = false;
            void Handler(object o, EventArgs e) { requerySuggested = true; }
            EventHandler handler = Handler;

            Assert.IsNotNull(handler);
            Assert.IsFalse(requerySuggested);

            System.Windows.Input.CommandManager.RequerySuggested += handler;
            CommandManagerHelper.InvalidateRequerySuggested("CommandManager Test");

            //await Task.Delay(100);
            Assert.IsNotNull(handler);
            Assert.IsTrue(requerySuggested, "\nCommandManager.RequerySuggested was not raised!");
            Console.WriteLine("- Success");
            System.Windows.Input.CommandManager.RequerySuggested -= handler;
        }

        [TestMethod()]
        public void SubscribeToCommandManagerTest()
        {
            //Create the command
            bool reacted = false;
            void Reaction(object sender, EventArgs e) => reacted = true;
            CommandManagerTest();
            var cmd = GetCommand();
            cmd.CanExecuteChanged += Reaction;

            //Ensure that it IS NOT RAISED when requery is suggested
            Assert.IsFalse(reacted);
            cmd.SubscribeToCommandManager = false;
            CommandManagerHelper.InvalidateRequerySuggested("Not Subscribed Test");
            Assert.IsFalse(reacted, "\nCanExecuteChanged was raised unexpectedly!");
            Console.WriteLine("- Success");

            // Ensure that setting value to TRUE causes it to listen to CommandManager
            cmd.SubscribeToCommandManager = true;
            CommandManagerHelper.InvalidateRequerySuggested("Subscribed Test");
            Assert.IsTrue(reacted, "\nCanExecuteChanged was never raised!");
            Console.WriteLine("- Success");

            // Ensure that setting value back to false works
            cmd.SubscribeToCommandManager = false;
            reacted = false;
            CommandManagerHelper.InvalidateRequerySuggested("Not Subscribed Test");
            Assert.IsFalse(reacted, "\nCanExecuteChanged was raised unexpectedly!");
            Console.WriteLine("- Success");
        }

        [TestMethod()]
        public void NotifyCanExecuteChangedTest()
        {
            bool reacted = false;
            void Reaction(object sender, EventArgs e) => reacted = true;

            Assert.IsFalse(reacted);
            var cmd = new ConcreteCommand() { SubscribeToCommandManager = false };
            cmd.CanExecuteChanged += Reaction;
            cmd.NotifyCanExecuteChanged();
            cmd.CanExecuteChanged -= Reaction;
            Assert.IsTrue(reacted);
        }

        [TestMethod()]
        public void NotifyCanExecuteChangedTest1()
        {
            bool reacted = false;
            void Reaction(object sender, EventArgs e) => reacted = true;

            Assert.IsFalse(reacted);
            var cmd = new ConcreteCommand() { SubscribeToCommandManager = false };
            cmd.CanExecuteChanged += Reaction;
            cmd.NotifyCanExecuteChanged(null, null);
            cmd.CanExecuteChanged -= Reaction;
            Assert.IsTrue(reacted);
        }
    }
}