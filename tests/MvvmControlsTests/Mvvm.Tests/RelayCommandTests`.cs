using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Input;
using RFBCodeWorks.Mvvm.Primitives;
using System;

namespace RFBCodeWorks.Mvvm.Tests
{
    [STATestClass]
    public class RelayCommandParameterTests : Primitives.Tests.CommandBaseTests
    {
        public RelayCommandParameterTests() { Initialize(); }

        protected override CommandBase GetCommand() => new RelayCommand<int>(CommandAction);

        private static bool WasExecuted = false;
        private static bool ErrorHandled = false;
        private static void CommandAction(int i) { WasExecuted = true; }
        private static void ThrowError(int i) { throw new Exception(); }
        private static bool CanExecute(int i) => i > 0;
        private static void HandleError(Exception e) => ErrorHandled = true;

        [TestInitialize]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public void Initialize()
        {
            WasExecuted = false;
            ErrorHandled = false;
        }

        [TestMethod]
        public void RelayCommandTest()
        {
            Assert.IsNotNull(new RelayCommand<int>(CommandAction));
        }

        [TestMethod]
        public void RelayCommandTest1()
        {
            var cmd = new RelayCommand<int>(ThrowError,HandleError);
            Assert.IsTrue(cmd.CanExecute(0), "\nCanExecute returns false unexpectedly");
            try
            {
                cmd.Execute(0);
            }
            finally
            {
                Assert.IsTrue(ErrorHandled, "\nError was not handled");
            }
        }

        [TestMethod]
        public void RelayCommandTest2()
        {
            var cmd = new RelayCommand<int>(CommandAction, CanExecute);
            Assert.IsTrue(cmd.CanExecute(1), "\nCanExecute returns false unexpectedly");
            Assert.IsFalse(cmd.CanExecute(0), "\nCanExecute returns true unexpectedly");
        }

        [TestMethod]
        public void RelayCommandTest3()
        {
            var cmd = new RelayCommand<int>(ThrowError, CanExecute, HandleError);
            Assert.IsTrue(cmd.CanExecute(1), "\nCanExecute returns false unexpectedly");
            try
            {
                cmd.Execute(1);
            }
            finally
            {
                Assert.IsTrue(ErrorHandled, "\nError was not handled");
            }
        }

        [TestMethod]
        public void CanExecuteTest()
        {
            var cmd = new RelayCommand<int>(CommandAction);
            Assert.IsTrue(cmd.CanExecute(1), "\nCanExecute returns false unexpectedly");
            cmd = new RelayCommand<int>(CommandAction, canExecute: CanExecute);
            Assert.IsFalse(cmd.CanExecute(0), "\nCanExecute returns true unexpectedly");
            Assert.IsTrue(cmd.CanExecute(1), "\nCanExecute returns false unexpectedly");
        }

        [TestMethod]
        public void ExecuteTest()
        {
            var cmd = new RelayCommand<int>(CommandAction);
            Assert.IsTrue(cmd.CanExecute(1), "\nCanExecute returns false unexpectedly");
            cmd.Execute(0);
            Assert.IsTrue(WasExecuted, "\nAction was not executed");
        }

    }
}