using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests
{

    [TestClass]
    public class ComboBoxGeneratorTests : SelectorGeneratorTests
    {
#if ROSLYN_311
        protected override string GetGeneratorName() => "ComboBox";
#else
        protected override IIncrementalGenerator GetGenerator() => new RefreshableComboBoxGeneratorRoslyn40();
#endif
        protected override void AssertImplementInterface(object instance) => Assert.IsInstanceOfType<IComboBox>(instance);
    }

    [TestClass]
    public class SelectorGeneratorTests
    {
#if ROSLYN_311
        protected virtual ISourceGenerator GetGenerator() => new RefreshableSelectorGeneratorRoslyn311();
#else
        protected virtual IIncrementalGenerator GetGenerator() => new RefreshableSelectorGeneratorRoslyn40();
#endif
        protected virtual string GetGeneratorName() => GetGenerator().GetType().Name;
        protected virtual void AssertImplementInterface(object instance) => Assert.IsInstanceOfType<ISelector>(instance);

        /// <summary>
        /// Helper object that wraps the constructed instance to work with it via reflection within the unit tests
        /// </summary>

        /// <param name="GeneratedObject"></param>
        protected record SelectorWrapper(object GeneratedObject, INamedTypeSymbol NamedTypeSymbol)
        {
            public string SelectorName { get; set; } = "NAME NOT SET";

            private PropertyInfo GetPropertyInfo(string propertyName)
            {
                var info = GeneratedObject.GetType().GetProperty(propertyName);
                Assert.IsNotNull(info, $"\n >> Unable to locate property name '{propertyName}' on generated button");
                return info;
            }
            public T GetProperty<T>(string propertyName) => Assert.IsInstanceOfType<T>(GetPropertyInfo(propertyName).GetValue(GeneratedObject, null));
            public void SetProperty<T>(string propertyName, T value) => GetPropertyInfo(propertyName).SetValue(GeneratedObject, value);

            public ISelector Selector => GetProperty<ISelector>(SelectorName);
        }

        protected SelectorWrapper GetSelector(Type typeToConstruct, Func<string, string>? getAdditionalText = null)
        {
            var fileContents = SourceGeneratorHelpers.ReadSourceText(typeToConstruct.Name);
            Assert.IsNotEmpty(fileContents);
            var generator = GetGenerator();
            string generateName = GetGeneratorName();

            CSharpCompilation compilation;
            ImmutableArray<Diagnostic> diagnostics;
            
            if (getAdditionalText is not null)
            {
                (compilation, diagnostics) = generator.RunSourceGenerator(fileContents, getAdditionalText(generateName));
            }
            else
            {
                (compilation, diagnostics) = generator.RunSourceGenerator(fileContents);
            }

            try
            {
                diagnostics.AssertNoDiagnostics();
                compilation.AssertCompilationHasNoErrors();
                var namedTypeSymbol = compilation.AssertGetTypeByName(typeToConstruct.FullName!);
                var constructedObj = compilation.ConstructInstance(typeToConstruct);
                string name = true switch
                {
                    true when generateName.Contains("Selector") => "ItemsSelector",
                    true when generateName.Contains("ComboBox") => "ItemsComboBox",
                    true when generateName.Contains("ListBox") => "ItemsListBox",
                    _ => throw new AssertFailedException($"Unexpected Generator Type : {generateName}")
                };
                return new SelectorWrapper(constructedObj, namedTypeSymbol) { SelectorName = name };
            }
            finally
            {
                compilation.PrintGeneratedTree(typeToConstruct.Name);
            }
        }

        

        [TestMethod]
        public void Test_Selector()
        {
            var wrapper = GetSelector(typeof(GeneratorInputs.Selector));
            var selector = Assert.IsInstanceOfType<Primitives.SelectorDefinition<int, int[], object>>(wrapper.Selector)
                .TestItemSourceChangedEvent()
                .TestIndexChanged()
                .TestSelectedItemChangedEvent()
                ;
            AssertImplementInterface(selector);
        }

        [TestMethod]
        public void Test_CollectionChanged()
        {
            var wrapper = GetSelector(typeof(GeneratorInputs.Selector_OnCollectionChanged),  GeneratorInputs.Selector_OnCollectionChanged.GetPartialText);
            var selector = Assert.IsInstanceOfType<Primitives.SelectorDefinition<int, int[], object>>(wrapper.Selector);

            Assert.IsFalse(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnCollectionChanged.ActionPerformed)));
            Assert.IsFalse(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnCollectionChanged.SelectorActionPerformed)));
            Assert.IsFalse(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnCollectionChanged.PartialMethodCalled)));
            Assert.IsFalse(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnCollectionChanged.CommandNotified)));
            _ = selector.Items;
            Assert.IsTrue(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnCollectionChanged.ActionPerformed)));
            Assert.IsTrue(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnCollectionChanged.SelectorActionPerformed)));
            Assert.IsTrue(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnCollectionChanged.PartialMethodCalled)));
            Assert.IsTrue(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnCollectionChanged.CommandNotified)));
        }

        [TestMethod]
        public void Test_CollectionChanged_SelectFirstItem()
        {
            var wrapper = GetSelector(typeof(GeneratorInputs.Selector_OnCollectionChanged_SelectFirstItem));
            var selector = Assert.IsInstanceOfType<ISelector>(wrapper.Selector);
            Assert.IsNull(selector.SelectedItem);
            _ = selector.Items;
            Assert.AreSame(selector.Items[0], selector.SelectedItem);
        }

        [TestMethod]
        public void Test_CollectionChanged_SelectLastItem()
        {
            var wrapper = GetSelector(typeof(GeneratorInputs.Selector_OnCollectionChanged_SelectLastItem));
            var selector = Assert.IsInstanceOfType<ISelector>(wrapper.Selector);
            Assert.IsNull(selector.SelectedItem);
            _ = selector.Items;
            Assert.AreSame(selector.Items[selector.Items.Count -1], selector.SelectedItem);
        }

        [TestMethod]
        public void Test_OnSelectionChanged()
        {
            var wrapper = GetSelector(typeof(GeneratorInputs.Selector_OnSelectionChanged), GeneratorInputs.Selector_OnSelectionChanged.GetPartialText);
            var selector = Assert.IsInstanceOfType<Primitives.SelectorDefinition<int, int[], object>>(wrapper.Selector);

            Assert.IsFalse(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnSelectionChanged.ActionPerformed)));
            Assert.IsFalse(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnSelectionChanged.SelectorActionPerformed)));
            Assert.IsFalse(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnSelectionChanged.PartialMethodCalled)));
            Assert.IsFalse(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnCollectionChanged.CommandNotified)));
            _ = selector.Items;
            Assert.IsFalse(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnSelectionChanged.ActionPerformed)));
            Assert.IsFalse(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnSelectionChanged.SelectorActionPerformed)));
            Assert.IsFalse(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnSelectionChanged.PartialMethodCalled)));
            Assert.IsFalse(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnCollectionChanged.CommandNotified)));
            selector.SelectedIndex = 2;
            Assert.IsTrue(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnSelectionChanged.ActionPerformed)));
            Assert.IsTrue(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnSelectionChanged.SelectorActionPerformed)));
            Assert.IsTrue(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnSelectionChanged.PartialMethodCalled)));
            Assert.IsTrue(wrapper.GetProperty<bool>(nameof(GeneratorInputs.Selector_OnCollectionChanged.CommandNotified)));
        }
    }
}
