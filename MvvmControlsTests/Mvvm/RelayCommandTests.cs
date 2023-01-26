using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm;
using RFBCodeWorks.Mvvm.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass()]
    public class RelayCommandTests : Primitives.Tests.CommandBaseTests
    {


        protected override CommandBase GetCommand() => new RelayCommand(new Action(() => { }));

        private static bool WasExecuted = false;
        private static bool ErrorHandled = false;
        private static void CommandAction() { WasExecuted = true; }
        private static void ThrowError() { throw new Exception(); }
        private static void HandleError(Exception e) => ErrorHandled = true;

        [TestInitialize]
        public void Initialize()
        {
            WasExecuted = false;
            ErrorHandled = false;
        }

        [TestMethod()]
        public void RelayCommandTest()
        {
            Assert.IsNotNull(new RelayCommand(CommandAction));
        }

        [TestMethod()]
        public void RelayCommandTest1()
        {
            var cmd = new RelayCommand(ThrowError,HandleError);
            Assert.IsTrue(cmd.CanExecute(), "\nCanExecute returns false unexpectedly");
            try
            {
                cmd.Execute();
            }
            finally
            {
                Assert.IsTrue(ErrorHandled, "\nError was not handled");
            }
        }

        [TestMethod()]
        public void RelayCommandTest2()
        {
            var cmd = new RelayCommand(CommandAction, canExecute: () => true);
            Assert.IsTrue(cmd.CanExecute(), "\nCanExecute returns false unexpectedly");
        }

        [TestMethod()]
        public void RelayCommandTest3()
        {
            var cmd = new RelayCommand(ThrowError, canExecute: () => true, HandleError);
            Assert.IsTrue(cmd.CanExecute(), "\nCanExecute returns false unexpectedly");
            try
            {
                cmd.Execute();
            }
            finally
            {
                Assert.IsTrue(ErrorHandled, "\nError was not handled");
            }
        }

        [TestMethod()]
        public void CanExecuteTest()
        {
            var cmd = new RelayCommand(CommandAction);
            Assert.IsTrue(cmd.CanExecute(), "\nCanExecute returns false unexpectedly");
            cmd = new RelayCommand(CommandAction, canExecute: () => false);
            Assert.IsFalse(cmd.CanExecute(), "\nCanExecute returns true unexpectedly");
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            var cmd = new RelayCommand(CommandAction);
            Assert.IsTrue(cmd.CanExecute(), "\nCanExecute returns false unexpectedly");
            cmd.Execute();
            Assert.IsTrue(WasExecuted, "\nAction was not executed");
        }

    }
}