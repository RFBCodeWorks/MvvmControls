using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests
{
    [TestClass]
    public class ButtonGeneratorTests
    {

#if ROSLYN_311
        private static RFBCodeWorks.Mvvm.ButtonGeneratorRoslyn311 GetGenerator() => new ButtonGeneratorRoslyn311();
#else
        private static RFBCodeWorks.Mvvm.ButtonGeneratorRoslyn40 GetGenerator() => new ButtonGeneratorRoslyn40();
#endif

        /// <summary>
        /// Helper object that wraps the constructed instance to work with it via reflection within the unit tests
        /// </summary>

        /// <param name="GeneratedObject"></param>
        record ButtonWrapper(object GeneratedObject, INamedTypeSymbol NamedTypeSymbol)
        {
            public string RunCommandName { get; set; } = "RunButton";
            
            private PropertyInfo GetPropertyInfo(string propertyName)
            {
                var info = GeneratedObject.GetType().GetProperty(propertyName);
                Assert.IsNotNull(info, $"Unable to locate property name {propertyName} on generated button");
                return info;
            }
            public T GetProperty<T>(string propertyName) => Assert.IsInstanceOfType<T>(GetPropertyInfo(propertyName).GetValue(GeneratedObject, null));
            public void SetProperty<T>(string propertyName, T value) => GetPropertyInfo(propertyName).SetValue(GeneratedObject, value);

            public IButtonDefinition Button => GetProperty<IButtonDefinition>(RunCommandName);
            public bool RunSuccess { get => GetProperty<bool>(nameof(RunSuccess)); set => SetProperty(nameof(RunSuccess), value); }
            public bool CanExecute { get => GetProperty<bool>(nameof(CanExecute)); set => SetProperty(nameof(CanExecute), value); }
            public int Min { get => GetProperty<int>(nameof(Min)); set => SetProperty(nameof(Min), value); }
            public int Max { get => GetProperty<int>(nameof(Max)); set => SetProperty(nameof(Max), value); }
            public int DelayPeriod { get => GetProperty<int>(nameof(DelayPeriod)); set => SetProperty(nameof(DelayPeriod), value); }
        }

        private static ButtonWrapper GetButtonWrapper(Type buttonType)
        {
            var fileContents = SourceGeneratorHelpers.ReadSourceText(buttonType.Name);
            Assert.IsNotEmpty(fileContents);
            var generator = GetGenerator();
            (CSharpCompilation compilation, var diagnostics) = generator.RunSourceGenerator(fileContents);
            try
            {
                diagnostics.AssertNoDiagnostics();
                compilation.AssertCompilationHasNoErrors();
                var namedTypeSymbol = compilation.AssertGetTypeByName(buttonType.FullName!);
                var constructedBtn = compilation.ConstructInstance(buttonType);
                Assert.IsNotNull(constructedBtn);
                var btn = new ButtonWrapper(constructedBtn, namedTypeSymbol);
                Assert.IsFalse(btn.RunSuccess);
                return btn;
            }
            finally
            {
                compilation.PrintGeneratedTree(buttonType.Name);
            }
        }

        [TestMethod]
        public void Test_Action()
        {
            var btn = GetButtonWrapper(typeof(GeneratorInputs.ButtonAction));
            var cmd = Assert.IsInstanceOfType<ButtonDefinition>(btn.Button);
            Assert.IsTrue(cmd.CanExecute(null));
            cmd.Execute();
            Assert.IsTrue(btn.RunSuccess);
        }

        [TestMethod]
        public void Test_ActionCanExecute()
        {
            var btn = GetButtonWrapper(typeof(GeneratorInputs.ButtonActionCanExecute));
            var cmd = Assert.IsInstanceOfType<ButtonDefinition>(btn.Button);
            btn.CanExecute = false;
            Assert.IsFalse(cmd.CanExecute(null));
            btn.CanExecute = true;
            Assert.IsTrue(cmd.CanExecute(null));
        }

        [TestMethod]
        public void Test_ActionCanExecuteInteger()
        {
            var btn = GetButtonWrapper(typeof(GeneratorInputs.ButtonCanExecuteInteger));
            var cmd = Assert.IsInstanceOfType<ButtonDefinition<int>>(btn.Button);
            btn.Min = 0;
            btn.Max = 100;
            Assert.IsFalse(((System.Windows.Input.ICommand)cmd).CanExecute(null));
            Assert.IsFalse(cmd.CanExecute(200));
            Assert.IsTrue(cmd.CanExecute(50));
        }

        [TestMethod]
        public void Test_Property_DisplayText()
        {
            var btn = GetButtonWrapper(typeof(GeneratorInputs.ButtonPropertyDisplayText));
            var cmd = Assert.IsInstanceOfType<IButtonDefinition>(btn.Button);
            Assert.AreEqual(GeneratorInputs.ButtonPropertyDisplayText.ExpectedDisplayText, cmd.DisplayText);
        }

        [TestMethod]
        public void Test_Property_PropertyName()
        {
            var btn = GetButtonWrapper(typeof(GeneratorInputs.ButtonPropertyName));
            btn.RunCommandName = GeneratorInputs.ButtonPropertyName.ExpectedPropertyName;
            var cmd = Assert.IsInstanceOfType<IButtonDefinition>(btn.Button);
        }

        [TestMethod]
        public void Test_Property_ToolTip()
        {
            var btn = GetButtonWrapper(typeof(GeneratorInputs.ButtonPropertyToolTip));
            var cmd = Assert.IsInstanceOfType<IButtonDefinition>(btn.Button);
            Assert.AreEqual(GeneratorInputs.ButtonPropertyToolTip.ExpectedToolTip, cmd.ToolTip);
        }

        [TestMethod]
        public async Task Test_Task()
        {
            var btn = GetButtonWrapper(typeof(GeneratorInputs.ButtonTask));
            var cmd = Assert.IsInstanceOfType<AsyncButtonDefinition>(btn.Button);
            Assert.IsFalse(cmd.CanBeCanceled);
            
            Assert.IsTrue(cmd.CanExecute());
            Assert.IsFalse(btn.RunSuccess);
            await cmd.ExecuteAsync();
            Assert.IsTrue(btn.RunSuccess);

            ICommand wpf = cmd;
            Assert.IsTrue(wpf.CanExecute(null));
        }

        [TestMethod]
        public async Task Test_TaskAllowConcurrentExecutionFalse()
        {
            var btn = GetButtonWrapper(typeof(GeneratorInputs.ButtonTaskAllowConcurrentExecutionsFalse));
            var cmd = Assert.IsInstanceOfType<AsyncButtonDefinition>(btn.Button);
            ICommand wpf = cmd;
            
            Assert.IsTrue(cmd.CanExecute());
            Assert.IsTrue(wpf.CanExecute(null));
            Assert.IsFalse(btn.RunSuccess);
            var t1 = cmd.ExecuteAsync();
            Assert.IsFalse(cmd.CanExecute());
            Assert.IsFalse(wpf.CanExecute(null));
            await t1;
            Assert.IsTrue(cmd.CanExecute());
            Assert.IsTrue(wpf.CanExecute(null));
            Assert.IsTrue(btn.RunSuccess);
        }

        [TestMethod]
        public async Task Test_TaskAllowConcurrentExecutionTrue()
        {
            var btn = GetButtonWrapper(typeof(GeneratorInputs.ButtonTaskAllowConcurrentExecutionsTrue));
            var cmd = Assert.IsInstanceOfType<AsyncButtonDefinition>(btn.Button);
            ICommand wpf = cmd;

            Assert.IsTrue(cmd.CanExecute());
            Assert.IsTrue(wpf.CanExecute(null));
            Assert.IsFalse(btn.RunSuccess);
            var t1 = cmd.ExecuteAsync();
            Assert.IsTrue(cmd.CanExecute());
            Assert.IsTrue(wpf.CanExecute(null));
            await t1;
            Assert.IsTrue(cmd.CanExecute());
            Assert.IsTrue(wpf.CanExecute(null));
            Assert.IsTrue(btn.RunSuccess);
        }

        [TestMethod]
        public async Task Test_TaskCanExecute()
        {
            var btn = GetButtonWrapper(typeof(GeneratorInputs.ButtonTaskCanExecute));
            var cmd = Assert.IsInstanceOfType<AsyncButtonDefinition>(btn.Button);
            ICommand wpf = cmd;

            btn.CanExecute = false;
            Assert.IsFalse(cmd.CanExecute());
            Assert.IsFalse(wpf.CanExecute(null));

            btn.CanExecute = true;
            Assert.IsTrue(cmd.CanExecute());
            Assert.IsTrue(wpf.CanExecute(null));
        }

        [TestMethod]
        public async Task Test_TaskCanExecuteInteger()
        {
            var btn = GetButtonWrapper(typeof(GeneratorInputs.ButtonTaskCanExecuteInteger));
            var cmd = Assert.IsInstanceOfType<AsyncButtonDefinition<int>>(btn.Button);
            ICommand wpf = cmd;

            btn.Min = 0;
            btn.Max = 100;
            Assert.IsFalse(wpf.CanExecute(null));
            Assert.IsFalse(cmd.CanExecute(200));

            Assert.IsTrue(cmd.CanExecute(50));
            Assert.IsTrue(wpf.CanExecute(50));
        }

        [TestMethod]
        public async Task Test_TaskIncludeCancelCommand()
        {
            var btn = GetButtonWrapper(typeof(GeneratorInputs.ButtonTaskIncludeCancelCommand));
            var cmd = Assert.IsInstanceOfType<AsyncButtonDefinition>(btn.Button);
            Assert.IsNotNull(btn.GetProperty<ICommand>("RunButtonCancelCommand"));
        }
    }
}
