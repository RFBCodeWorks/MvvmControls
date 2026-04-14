using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Specialized;
using System;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass]
    public class ButtonTests
    {
        [TestMethod]
        [Timeout(100, CooperativeCancellation = false)]
        public void Test_AsyncButton_CanExecute()
        {
            bool canExecute = false;
            var btn = new AsyncButtonDefinition(() => Task.CompletedTask, () => canExecute);
            Assert.IsFalse(btn.CanExecute());
            canExecute = true;
            Assert.IsTrue(btn.CanExecute());
        }

        [TestMethod]
        [Timeout(100, CooperativeCancellation = false)]
        public void Test_AsyncButtonParameter_CanExecute()
        {
            bool canExecute = false;
            Task AsyncFunc(int x) => Task.CompletedTask;
            bool CanExecute(int x) => canExecute;
            var btn = new AsyncButtonDefinition<int>(AsyncFunc, CanExecute);
            
            Assert.IsFalse(btn.CanExecute(6));
            canExecute = true;
            Assert.IsTrue(btn.CanExecute(10));
        }

        [TestMethod]
        [Timeout(100, CooperativeCancellation = false)]
        public void Test_Button_CanExecute()
        {
            bool canExecute = false;
            var btn = new ButtonDefinition(() => { }, () => canExecute);
            Assert.IsFalse(btn.CanExecute(null));
            canExecute = true;
            Assert.IsTrue(btn.CanExecute(null));
        }

        [TestMethod]
        [Timeout(100, CooperativeCancellation = false)]
        public void Test_ButtonParameter_CanExecute()
        {
            bool canExecute = false;
            var btn = new ButtonDefinition<int>((x) => { }, (x) => canExecute);
            Assert.IsFalse(btn.CanExecute(1));
            canExecute = true;
            Assert.IsTrue(btn.CanExecute(2));
        }

        [TestMethod]
        [Timeout(100, CooperativeCancellation = false)]
        public void Test_TwoStateButton_CanExecute_Default()
        {
            bool canExecute = false;
            var btn = new TwoStateButton()
            {
                DefaultAction = () => throw new InvalidOperationException("Executed Default Action"),
                AlternateAction = () => throw new InvalidOperationException("Executed Alternate Action"),
                DefaultActionCanExecute = () => canExecute,
                AlternateActionCanExecute = () => throw new InvalidOperationException("Called Alternate CanExecute"),
            };
            btn.IsDefaultState = true;
            Assert.IsFalse(btn.CanExecute());
            canExecute = true;
            Assert.IsTrue(btn.CanExecute());
        }

        [TestMethod]
        [Timeout(100, CooperativeCancellation = false)]
        public void Test_TwoStateButton_CanExecute_Alternate()
        {
            bool canExecuteAlt = false;
            var btn = new TwoStateButton()
            {
                DefaultAction = () => throw new InvalidOperationException("Executed Default Action"),
                AlternateAction = () => throw new InvalidOperationException("Executed Alternate Action"),
                DefaultActionCanExecute = () => throw new InvalidOperationException("Called Default CanExecute"),
                AlternateActionCanExecute = () => canExecuteAlt,
            };
            btn.IsDefaultState = false;
            Assert.IsFalse(btn.CanExecute());
            canExecuteAlt = true;
            Assert.IsTrue(btn.CanExecute());
        }

        [TestMethod]
        [Timeout(100, CooperativeCancellation = false)]
        public void Test_TwoStateButton_Execute_Default()
        {
            bool executed = false;
            var btn = new TwoStateButton()
            {
                DefaultAction = () => executed = true,
                AlternateAction = () => throw new InvalidOperationException("Executed Alternate Action"),
                DefaultActionCanExecute = () => throw new InvalidOperationException("Called Default CanExecute"),
                AlternateActionCanExecute = () => throw new InvalidOperationException("Called Alternate CanExecute"),
            };
            btn.IsDefaultState = true;
            Assert.IsFalse(executed);
            btn.Execute();
            Assert.IsTrue(executed);
        }

        [TestMethod]
        [Timeout(100, CooperativeCancellation =false)]
        public void Test_TwoStateButton_Execute_Alternate()
        {
            bool executed = false;
            var btn = new TwoStateButton()
            {
                DefaultAction = () => throw new InvalidOperationException("Executed Default Action"),
                AlternateAction = () => executed = true,
                DefaultActionCanExecute = () => throw new InvalidOperationException("Called Default CanExecute"),
                AlternateActionCanExecute = () => throw new InvalidOperationException("Called Alternate CanExecute"),
            };
            btn.IsDefaultState = false;
            Assert.IsFalse(executed);
            btn.Execute();
            Assert.IsTrue(executed);
        }
    }
}
