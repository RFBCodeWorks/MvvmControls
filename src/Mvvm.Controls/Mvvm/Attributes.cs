using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#nullable enable
#nullable disable warnings

#if GENERATORS
#pragma warning disable CS1574 // disable "Can Not Resolve" specifically for Generators projects
#endif

namespace RFBCodeWorks.Mvvm
{


    /// <summary>
    /// Adds the <see cref="RFBCodeWorks.Mvvm.IViewModel"/> interface to a class.
    /// </summary>    
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
#if GENERATORS
    internal sealed class IViewModelAttribute : Attribute
#else
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class IViewModelAttribute : Attribute
#endif
    {

    }

    /// <summary>
    /// Turns a method into a <see cref="Mvvm.ButtonDefinition"/>.
    /// <br/> Functions similar to <see cref="CommunityToolkit.Mvvm.Input.RelayCommand"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
#if GENERATORS
    internal sealed class ButtonAttribute : Attribute
#else
    public sealed class ButtonAttribute : Attribute
#endif
    {
        /// <summary>
        /// A custom name to use for the generated property.
        /// <para/> Otherwise, the name will be generated based on the method name.
        /// <br/> ex: DoSomething() -> DoSomethingButton
        /// </summary>
        public string? PropertyName { get; set; }

        /// <summary>
        /// The text to display as the button content
        /// </summary>
        public string? DisplayText { get; set; }

        /// <summary>
        /// The tooltip text for the button
        /// </summary>
        public string? Tooltip { get; set; }

        /// <summary>
        /// Name of a method/property (bool returning) used to enable/disable this command.
        /// </summary>
        public string? CanExecute { get; set; }

        /// <summary>
        /// If true, allows multiple concurrent executions of an async method.
        /// <br/> Default = <see langword="false"/>.
        /// </summary>
        public bool AllowConcurrentExecutions { get; set; }

        /// <summary>
        /// If true (async only), exceptions flow to the TaskScheduler instead of rethrowing.
        /// <br/> Default = <see langword="false"/>.
        /// </summary>
        public bool FlowExceptionsToTaskScheduler { get; set; }

        /// <summary>
        /// If true (async only), also generates a “Cancel” command for the operation.
        /// <br/> Default = <see langword="false"/>.
        /// </summary>
        public bool IncludeCancelCommand { get; set; }

    }

    /// <summary>
    /// Apply to a method that returns IList&lt;T&gt; to generate a <see cref="RefreshableComboBoxDefinition{T, V}"/> property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
#if GENERATORS
    internal sealed class ComboBoxAttribute : SelectorAttribute
#else
    public sealed class ComboBoxAttribute : SelectorAttribute
#endif
    { }

    /// <summary>
    /// Apply to a method that returns IList&lt;T&gt; to generate a <see cref="RefreshableListBoxDefinition{T, V}"/> property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
#if GENERATORS
    internal sealed class ListBoxAttribute : SelectorAttribute
#else
    public sealed class ListBoxAttribute : SelectorAttribute
#endif
    { }

    /// <summary>
    /// Apply to a method that returns IList&lt;T&gt; to generate a <see cref="RefreshableSelector{T, TList, TSelectedValue}"/> property.
    /// </summary>
    /// <remarks>The Selector does not have specialized properties that a listbox or combobox would have, such as <see cref="IComboBox.IsDropDownOpen"/></remarks>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
#if GENERATORS
    internal class SelectorAttribute : Attribute
#else
    public class SelectorAttribute : Attribute
#endif
    {
        /// <summary>
        /// If true, get the collection the first time the collection property is requested.
        /// <br/> default = true
        /// </summary>
        public bool RefreshOnInitialize { get; set; }

        /// <summary>
        /// Name of a parameterless method returning bool that determines if Refresh can run.
        /// </summary>
        public string CanRefresh { get; set; }

        /// <summary>
        /// The type of <see cref="ISelector.SelectedValue"/> to specify. Default is 'Object'
        /// <para>
        /// For example, assuming this attribute is on a method that returns FileInfo[], specifying 'string' would result in :
        /// <br/>RefreshableListBox&lt;FileInfo, FileInfo[], string&gt;
        /// </para>
        /// </summary>
        public Type? SelectedValueType { get; set; }

        /// <summary>
        /// The tooltip text for the Selector
        /// </summary>
        public string? ToolTip { get; set; }

        /// <summary>
        /// The DisplayMemberPath for the Selector
        /// </summary>
        public string? DisplayMemberPath { get; set; }

        /// <summary>
        /// The SelectedValuePath for the Selector
        /// </summary>
        public string? SelectedValuePath { get; set; }
        
        /// <summary>
        /// The name to use for the public property
        /// <br/>If supplied, this is the name that will be used instead of the auto-generated name (such as FilesSelector)
        /// </summary>
        public string? PropertyName { get; set; }
    }

    /// <summary>
    /// Notify any IRelayCommands Generate or call a specific method when the <see cref="ISelector.SelectedItem"/> changes
    /// </summary>
    /// <remarks>
    /// Can be applied to :
    /// <br/> - Methods with either <see cref="ComboBoxAttribute"/> or <see cref="ListBoxAttribute"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
#if GENERATORS
    internal sealed class OnSelectionChangedAttribute : Attribute
#else
    public sealed class OnSelectionChangedAttribute : Attribute
#endif
    {
        /// <inheritdoc cref="OnSelectionChangedAttribute"/>
        /// <param name="commandsToNotify"><inheritdoc cref="CommandsToNotify" path="*"/></param>
        public OnSelectionChangedAttribute(params string[] commandsToNotify)
        {
            CommandsToNotify = commandsToNotify;
        }

        /// <summary>
        /// Names of ICommands to raise when the selection changes.
        /// </summary>
        /// <remarks>
        /// <para>Generates : 
        /// <br/>( ) => 
        /// <br/>{ 
        /// <br/>commandName.NotifyCanExecuteChanged();
        /// <br/>commandName2.NotifyCanExecuteChanged();
        /// <br/>}
        /// </para>
        /// </remarks>
        public string[] CommandsToNotify { get; }

        /// <summary>
        /// [Optional] name of a parameterless method to call when the <see cref="ISelector.SelectedItem"/> changes.
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// [Optional] name of a method that accepts a <see cref="ISelector"/> as the parameter.
        /// <para/> This action will be performed when the <see cref="ISelector.SelectedItem"/> changes.
        /// </summary>
        /// <remarks>
        /// Example : 
        /// <br/>SelectorAction = <see langword="nameof"/>(DoSomething)
        /// <br/>void DoSomething(ISelector selector);
        /// </remarks>
        public string? SelectorAction { get; set; }
    }

    /// <summary>
    /// Notify IRelayCommands when the <see cref="IItemSource.Items"/> property is updated.
    /// <br/>Alternatively, method via <see cref="Action"/> or <see cref="SelectorAction"/>
    /// </summary>
    /// <remarks>
    /// Can be applied to :
    /// <br/> - Methods with either <see cref="ComboBoxAttribute"/>, <see cref="ListBoxAttribute"/>, or <see cref="SelectorAttribute"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
#if GENERATORS
    internal sealed class OnCollectionChangedAttribute : Attribute
#else
    public sealed class OnCollectionChangedAttribute : Attribute
#endif
    {
        /// <inheritdoc cref="OnCollectionChangedAttribute"/>
        /// <param name="commandsToNotify"><inheritdoc cref="CommandsToNotify" path="*"/></param>
        public OnCollectionChangedAttribute(params string[] commandsToNotify)
        {
            CommandsToNotify = commandsToNotify;
            Action = "";
        }

        /// <summary>
        /// Names of ICommands to raise when the item source changes.
        /// </summary>
        /// <inheritdoc cref="OnSelectionChangedAttribute.CommandsToNotify"/>
        public string[] CommandsToNotify { get; }

        /// <summary>
        /// [Optional] name of a parameterless method to call when the <see cref="IItemSource.Items"/> property changes.
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// [Optional] name of a method that accepts a <see cref="ISelector"/> as the parameter.
        /// <para/> This action will be performed when the <see cref="ISelector.SelectedItem"/> changes.
        /// </summary>
        /// <remarks>
        /// Example : 
        /// <br/>SelectorAction = <see langword="nameof"/>(DoSomething)
        /// <br/>void DoSomething(ISelector selector);
        /// <para/>
        /// Example 2:
        /// <br/>SelectorAction = "RFBCodeWorks.Mvvm.SelectorUtilities.TrySelectFirstItem"
        /// <br/>SelectorAction = "RFBCodeWorks.Mvvm.SelectorUtilities.TrySelectLastItem"
        /// </remarks>
        public string? SelectorAction { get; set; }
    }

    /// <summary>
    /// Issues a refresh command to the specified selectors.
    /// <para>
    /// When used on an observable property, generates OnValueChanged partial method to invoke the refresh command of a refreshable selector.
    /// </para>
    /// <para>
    /// When used on a method that generates a refreshable selector, the downstream selector will be refreshed after the selection has been changed.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
#if GENERATORS
    internal sealed class TriggersRefreshAttribute : Attribute
#else
    public sealed class TriggersRefreshAttribute : Attribute
#endif
    {
        /// <inheritdoc cref="TriggersRefreshAttribute"/>
        public TriggersRefreshAttribute(params string[] selectorNames)
        {
            SelectorNames = selectorNames;
        }

        /// <summary>
        /// Gets the list of selector names that should be refreshed.
        /// </summary>
        public string[] SelectorNames { get; }
    }
}

#pragma warning restore CS157