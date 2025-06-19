using System;
using System.Threading.Tasks;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Turns a method into a <see cref="Mvvm.ButtonDefinition"/>.
    /// <br/> Functions similar to <see cref="CommunityToolkit.Mvvm.Input.RelayCommand"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public  sealed partial class ButtonAttribute : Attribute
    {
        /// <summary>
        /// Human-friendly name for the command (e.g. button text).
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// Name of a method/property (bool returning) used to enable/disable this command.
        /// </summary>
        public string? CanExecute { get; set; }

        /// <summary>
        /// If true, allows multiple concurrent executions of an async method.
        /// </summary>
        public bool AllowConcurrentExecutions { get; set; }

        /// <summary>
        /// If true (async only), exceptions flow to the TaskScheduler instead of rethrowing.
        /// </summary>
        public bool FlowExceptionsToTaskScheduler { get; set; }

        /// <summary>
        /// If true (async only), also generates a “Cancel” command for the operation.
        /// </summary>
        public bool IncludeCancelCommand { get; set; }

        [CommunityToolkit.Mvvm.Input.RelayCommand(FlowExceptionsToTaskScheduler =true, IncludeCancelCommand = true, AllowConcurrentExecutions = true)]
        System.Threading.Tasks.Task Test(System.Threading.CancellationToken token)
        {
            return Task.CompletedTask;
        }

    }

    /// <summary>
    /// Apply to a method that returns IList&lt;T&gt; to generate a <see cref="RefreshableComboBoxDefinition{T, V}"/> binding.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ComboBoxAttribute : Attribute
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
        /// <br/>RefreshableComboBox&lt;FileInfo, FileInfo[], string&gt;
        /// </para>
        /// </summary>
        public Type? SelectedValueType { get; set; }
    }

    /// <summary>
    /// Apply to a method that returns IList&lt;T&gt; to generate a <see cref="RefreshableListBoxDefinition{T, V}"/> binding.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ListBoxAttribute : Attribute
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
    }

    /// <summary>
    /// Generate or call a specific method when the <see cref="ISelector.SelectedItem"/> changes
    /// </summary>
    /// <remarks>
    /// Can be applied to :
    /// <br/> - Methods with either <see cref="ComboBoxAttribute"/> or <see cref="ListBoxAttribute"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class OnSelectionChangedAttribute : Attribute
    {
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
        /// Alternatively, name of a void method to call on selection change.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string MethodName { get; set; }

        /// <inheritdoc cref="OnSelectionChangedAttribute"/>
        /// <param name="commandsToNotify"><inheritdoc cref="CommandsToNotify" path="*"/></param>
        public OnSelectionChangedAttribute(params string[] commandsToNotify)
        {
            CommandsToNotify = commandsToNotify;
        }
    }

    /// <summary>
    /// Generate or call a specific method when the <see cref="IItemSource.Items"/> changes
    /// </summary>
    /// <remarks>
    /// Can be applied to :
    /// <br/> - Methods with either <see cref="ComboBoxAttribute"/> or <see cref="ListBoxAttribute"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OnItemSourceChangedAttribute : Attribute
    {
        /// <summary>
        /// Names of ICommands to raise when the item source changes.
        /// </summary>
        /// <inheritdoc cref="OnSelectionChangedAttribute.CommandsToNotify"/>
        public string[] CommandsToNotify { get; }

        /// <summary>
        /// Alternatively, name of a void method to call on item-source change.
        /// </summary>
        public string MethodName { get; set; }

        /// <inheritdoc cref="OnItemSourceChangedAttribute"/>
        /// <param name="commandsToNotify"><inheritdoc cref="CommandsToNotify" path="*"/></param>
        public OnItemSourceChangedAttribute(params string[] commandsToNotify)
        {
            CommandsToNotify = commandsToNotify;
            MethodName = "";
        }
    }
}