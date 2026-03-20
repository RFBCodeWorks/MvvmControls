using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass]
    public class ObjectViewModelTests
    {
        /// <summary>
        /// Run through the various tests for this ICommand
        /// </summary>
        private void GenericCommandTests(TestViewModel ViewModel, IRelayCommand<string> TestCommand)
        {
            ViewModel.Model = new ObjectModel(false);
            void Run() => TestCommand.Execute(false);
            //Verify an exception declaring an invalid type was passed into the method that took a parameter
            Assert.Throws<ArgumentException>(Run);
            
            //Verify No Exceptions Thrown for Valid Input
            TestCommand.Execute(null);
            TestCommand.Execute("TEST");
            
            CommandTests(ViewModel, TestCommand);
        }

        /// <summary>
        /// Run through the various tests for this ICommand
        /// </summary>
        private void CommandTests(TestViewModel ViewModel, IRelayCommand TestCommand)
        {
            //Command should not be able to execute without the object being set
            ViewModel.Model = null!;
            Assert.IsFalse(TestCommand.CanExecute(null));

            //Set the object
            ViewModel.Model = new ObjectModel(true);
            Assert.IsNull(ViewModel.Model.CommandResult);

            //Command should now be able to run & Test Result
            Assert.IsTrue(TestCommand.CanExecute(null));
            TestCommand.Execute(null);
            Assert.IsTrue(ViewModel.Model?.CommandResult);

            //Set back to null and verify CanExecute returns false
            ViewModel.Model = null!;
            Assert.IsFalse(TestCommand.CanExecute(null));

            //Set Expected result to false  and try again
            ViewModel.Model = new ObjectModel(false);
            Assert.IsNull(ViewModel.Model?.CommandResult);
            TestCommand.Execute(null);
            Assert.IsFalse(ViewModel.Model?.CommandResult);
        }

        [TestMethod]
        public void ObjectViewModel_EventTests()
        {
            var ViewModel = new TestViewModel();

            //Set the value
            bool EventRaised = false;
            ViewModel.PropertyChanged += (o, e) => EventRaised = true;
            ViewModel.Model = new ObjectModel(true);
            Assert.IsTrue(EventRaised); EventRaised = false;
            
            //Validate the ToolTip gets modified AND that it raises an event
            ViewModel.FromMethodName_ObjectMethodGeneric.PropertyChanged += (o, e) => EventRaised = true;
            string tt = "ToolTipTest";
            Assert.AreNotEqual(tt, ViewModel.FromMethodName_ObjectMethodGeneric.ToolTip);
            ViewModel.FromMethodName_ObjectMethodGeneric.ToolTip = tt;
            Assert.IsTrue(EventRaised); EventRaised = false;
            Assert.AreEqual(tt, ViewModel.FromMethodName_ObjectMethodGeneric.ToolTip);
        }

        /// <summary>
        /// Test that this method result changes when object reference changes
        /// </summary>
        [TestMethod]
        public void CommandFactory_FromMethodName()
        {
            var ViewModel = new TestViewModel();
            CommandTests(ViewModel, ViewModel.FromMethodName_ObjectMethod);
        }

        [TestMethod]
        public void CommandFactory_FromMethodNameGeneric()
        {
            var ViewModel = new TestViewModel();
            GenericCommandTests(ViewModel, ViewModel.FromMethodName_ObjectMethodGeneric);
        }

        /// <summary>
        /// Test that this method result changes when object reference changes
        /// </summary>
        [TestMethod]
        public void CommandFactory_FromAction()
        {
            var ViewModel = new TestViewModel();
            CommandTests(ViewModel, ViewModel.FromAction_ObjectMethod);
        }

        [TestMethod]
        public void CommandFactory_FromActionGeneric()
        {
            var ViewModel = new TestViewModel();
            GenericCommandTests(ViewModel, ViewModel.FromAction_ObjectMethodGeneric);
        }

        [TestMethod]
        public void CommandFactory_PropertyMethod()
        {
            var ViewModel = new TestViewModel();
            CommandTests(ViewModel, ViewModel.FromAction_PropertyMethod);
        }

        [TestMethod]
        public void CommandFactory_PropertMethodGeneric()
        {
            var ViewModel = new TestViewModel();
            GenericCommandTests(ViewModel, ViewModel.FromAction_PropertyMethodGeneric);
        }

        [TestMethod]
        public void CommandFactory_FromMethodNameAsync()
        {
            var ViewModel = new TestViewModel();
            CommandTests(ViewModel, ViewModel.FromTaskMethod);
        }

        [TestMethod]
        public void CommandFactory_FromMethodNameAsyncGeneric()
        {
            var ViewModel = new TestViewModel();
            GenericCommandTests(ViewModel, ViewModel.FromTaskMethod2);
        }

    }

    public class TestViewModel : Mvvm.ComponentModel.ObjectViewModel<ObjectModel>
    {
        public static TestViewModel ViewModel { get; } = new TestViewModel();

        public TestViewModel() : base()
        {
            FromMethodName_ObjectMethod = this.CommandFactory.FromMethodName(nameof(this.Model.SetResult));
            FromMethodName_ObjectMethodGeneric = this.CommandFactory.FromMethodName<string>(nameof(this.Model.SetResult));

            FromAction_ObjectMethod = this.CommandFactory.FromAction(new Action( () => this.Model.SetResult()));
            FromAction_ObjectMethodGeneric = this.CommandFactory.FromAction<string>(new Action<string>((s) => this.Model.SetResult(s)));
            
            FromAction_PropertyMethod = this.CommandFactory.FromAction(new Action(() => Model.TestProperty.TestState.SetState()));
            FromAction_PropertyMethodGeneric = this.CommandFactory.FromAction<string>(new Action<string>((s) => this.Model.TestProperty.TestState.SetState(s)));

            FromTaskMethod = this.CommandFactory.FromMethodNameAsync(nameof(Model.TaskMethod));
            FromTaskMethod2 = this.CommandFactory.FromMethodNameAsync<string>(nameof(Model.TaskMethod));
        }

        public ButtonDefinition FromMethodName_ObjectMethod { get; }
        public ButtonDefinition<string> FromMethodName_ObjectMethodGeneric { get; }

        public ButtonDefinition FromAction_ObjectMethod { get; }
        public ButtonDefinition<string> FromAction_ObjectMethodGeneric { get; }

        public ButtonDefinition FromAction_PropertyMethod { get; }
        public ButtonDefinition<string> FromAction_PropertyMethodGeneric { get; }

        public AsyncButtonDefinition FromTaskMethod { get; }
        public AsyncButtonDefinition<string> FromTaskMethod2 { get; }

    }

    public class ObjectModel
    {
        public ObjectModel(bool result) { ExpectedCommandResult = result; TestProperty = new PropClass2(result, this); }
        public PropClass2 TestProperty { get; }

        public bool ExpectedCommandResult { get; }
        public bool? CommandResult { get; private set; } = null;
        public void SetResult() => CommandResult = ExpectedCommandResult;
        public void SetResult(string test) => CommandResult = ExpectedCommandResult;

        public Task TaskMethod() => PerformTask();
        public Task TaskMethod(string test) => PerformTask();

        private Task PerformTask()
        {
            SetResult();
            return Task.CompletedTask;
        }

    }

    public class PropClass2
    {
        public PropClass2(bool state, ObjectModel parent) { Parent = parent; TestState = new PropClass(state, this); }
        public PropClass TestState { get; }
        public ObjectModel Parent { get; }
    }

    public class PropClass
    {
        public PropClass(bool state, PropClass2 parent) { ExpectedState = state; Parent = parent; }
        PropClass2 Parent { get; }
        public bool State { get => (bool)(Parent?.Parent?.CommandResult ?? throw new NullReferenceException()); private set { Parent.Parent.SetResult(); } }
        public bool ExpectedState { get; }
        public void SetState() => State = ExpectedState;
        public void SetState(string obj) => State = ExpectedState;
    }
}