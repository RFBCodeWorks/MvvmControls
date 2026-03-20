using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Tests;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace RFBCodeWorks.Mvvm.Primitives.Tests
{
    [STATestClass]
    public class PrimitiveCommandBaseTests : CommandBaseTests
    {
        protected override CommandBase GetCommand() => new ConcreteCommand();
        public class ConcreteCommand : CommandBase
        {
            public ConcreteCommand() : base()
            {
            }
        }
    }

    /// <summary>
    /// Base for shared functionality of <see cref="CommandBase"/> testing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CommandBaseTests
    {
        protected abstract CommandBase GetCommand();

        [STATestMethod]
        public async Task SubscribeToCommandManagerTest()
        {
            //Create the command
            bool reacted = false;
            void Reaction(object? sender, EventArgs e) => reacted = true;
            
            var cmd = GetCommand();
            cmd.CanExecuteChanged += Reaction;

            //Ensure that it IS NOT RAISED when requery is suggested
            Assert.IsFalse(reacted);
            cmd.SubscribeToCommandManager = false;
            await CommandManagerHelper.InvalidateRequerySuggested("Not Subscribed Test");
            Assert.IsFalse(reacted, "\nCanExecuteChanged was raised unexpectedly!");
            Console.WriteLine("- Success");

            // Ensure that setting value to TRUE causes it to listen to CommandManager
            cmd.SubscribeToCommandManager = true;
            await CommandManagerHelper.InvalidateRequerySuggested("Subscribed Test");
            Assert.IsTrue(reacted, "\nCanExecuteChanged was never raised!");
            Console.WriteLine("- Success");

            // Ensure that setting value back to false works
            cmd.SubscribeToCommandManager = false;
            reacted = false;
            await CommandManagerHelper.InvalidateRequerySuggested("Not Subscribed Test");
            Assert.IsFalse(reacted, "\nCanExecuteChanged was raised unexpectedly!");
            Console.WriteLine("- Success");
        }

        [TestMethod]
        public void NotifyCanExecuteChangedTest()
        {
            bool reacted = false;
            void Reaction(object? sender, EventArgs e) => reacted = true;

            Assert.IsFalse(reacted);
            var cmd = GetCommand();
            cmd.SubscribeToCommandManager = false;
            cmd.CanExecuteChanged += Reaction;
            cmd.NotifyCanExecuteChanged();
            cmd.CanExecuteChanged -= Reaction;
            Assert.IsTrue(reacted, $"\n > {nameof(IRelayCommand.CanExecuteChanged)} was not raised.");
        }

        [TestMethod]
        public void NotifyCanExecuteChangedEventHandlerTest()
        {
            bool reacted = false;
            void Reaction(object? sender, EventArgs e) => reacted = true;

            Assert.IsFalse(reacted);
            var cmd = GetCommand();
            cmd.SubscribeToCommandManager = false;
            cmd.CanExecuteChanged += Reaction;
            cmd.NotifyCanExecuteChanged(null, EventArgs.Empty);
            cmd.CanExecuteChanged -= Reaction;
            Assert.IsTrue(reacted, $"\n > {nameof(IRelayCommand.CanExecuteChanged)} was not raised.");
        }
    }
}