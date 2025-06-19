using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Interface all ComboBoxDefinitions should implement to be assignable via AttachedProperty
    /// <br/> Inherits: <see cref="ISelector"/>
    /// </summary>
    public interface IComboBox : ISelector
    {
        /// <inheritdoc cref="System.Windows.Controls.ComboBox.IsDropDownOpen"/>
        /// <remarks>This property can be used to force the drop-down open from the ViewModel</remarks>
        bool IsDropDownOpen { get; set; }

        /// <inheritdoc cref="ComboBoxDefinition{T, TList,  TSelectedValue}.IsReadOnly"/>
        bool IsReadOnly { get; set; }

        /// <inheritdoc cref="ComboBoxDefinition{T, TList,  TSelectedValue}.IsEditable"/>
        bool IsEditable { get; set; }
    }

    /// <summary>
    /// Interface all ComboBoxDefinitions of some type implement
    /// <para/>Inherits: 
    /// <br/>  - <see cref="IComboBox"/>
    /// <br/>  - <see cref="ISelector"/>
    /// <br/>  - <see cref="ISelector{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IComboBox<T> : ISelector<T>, IComboBox { }

    /// <summary>
    /// The base ComboBoxDefinition
    /// </summary>
    /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}"/>
    public partial class ComboBoxDefinition<T, TList, TSelectedValue> : Primitives.SelectorDefinition<T, TList, TSelectedValue>, IComboBox, IComboBox<T>
        where TList : IList<T>
    {
        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition()"/>
        public ComboBoxDefinition() { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition()"/>
        public ComboBoxDefinition(TList collection) :base(collection) { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ComboBoxDefinition(Action onItemSourceChanged = null, Action onSelectionChanged = null, TList collection = default) : base(onItemSourceChanged, onSelectionChanged, collection) { }

        private bool _isEditable;

        /// <inheritdoc cref="System.Windows.Controls.ComboBox.Text"/>
        /// <remarks>
        /// When the IsEditable property is true, setting this property places initial text entered in the text box. When IsEditable is false, setting this value has no effect.
        /// <br/>
        /// See the link below for more information on this interaction: <br/>
        /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox?view=windowsdesktop-6.0"/>
        /// </remarks>
        [ObservableProperty]
        private string _text;

        /// <inheritdoc/>
        [ObservableProperty]
        private bool _isDropDownOpen;

        /// <inheritdoc cref="System.Windows.Controls.ComboBox.IsEditable"/>
        /// <remarks>
        /// See the link below for more information on this interaction: <br/>
        /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox?view=windowsdesktop-6.0"/>
        /// </remarks>
        public virtual bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                if (_isEditable != value)
                {
                    OnPropertyChanging(EventArgSingletons.IsEditable);
                    _isEditable = value;
                    OnPropertyChanged(EventArgSingletons.IsEditable);
                }
            }
        }

        /// <returns>
        /// true if the ComboBox is read-only; otherwise, false. <br/>
        /// The default for the control is false, the default for this definition is true.
        /// </returns>
        /// <inheritdoc cref="System.Windows.Controls.ComboBox.IsReadOnly"/>
        /// <remarks>
        /// See the link below for more information on this interaction: <br/>
        /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox?view=windowsdesktop-6.0"/>
        /// </remarks>
        [ObservableProperty]
        private bool _isReadOnly = true;

    }

    /// <summary>
    /// A Simple ComboBox Definition that only specifies the enumerated type
    /// </summary>
    /// <inheritdoc cref="ComboBoxDefinition{T, TList, TSelectedValue}"/>
    public class ComboBoxDefinition<T> : ComboBoxDefinition<T, IList<T>, object> 
    {
        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition()"/>
        public ComboBoxDefinition() { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ComboBoxDefinition(IList<T> collection) : this(null, null, collection) { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ComboBoxDefinition(params T[] collection) : this(null, null, collection) { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ComboBoxDefinition(Action onItemSourceChanged = null, Action onSelectionChanged = null, params T[] collection) : base(onItemSourceChanged, onSelectionChanged, collection ?? Array.Empty<T>()) { }


        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ComboBoxDefinition(Action onItemSourceChanged = null, Action onSelectionChanged = null, IList<T> collection = default) : base(onItemSourceChanged, onSelectionChanged, collection ?? Array.Empty<T>()) { }
    }

    /// <summary>
    /// A generic bindable definition for a ComboBox whose ItemSource is any IEnumerable object, that expects a SelectedValue of a specific type
    /// </summary>
    /// <inheritdoc cref="ComboBoxDefinition{T, TList, TSelectedValue}"/>
    public class ComboBoxDefinition<T, TSelectedValue> : ComboBoxDefinition<T, IList<T>, TSelectedValue> 
    {
        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition()"/>
        public ComboBoxDefinition() { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ComboBoxDefinition(IList<T> collection) : this(null, null, collection) { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ComboBoxDefinition(params T[] collection) : this(null, null, collection) { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ComboBoxDefinition(Action onItemSourceChanged = null, Action onSelectionChanged = null, params T[] collection) : base(onItemSourceChanged, onSelectionChanged, collection ?? Array.Empty<T>()) { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ComboBoxDefinition(Action onItemSourceChanged = null, Action onSelectionChanged = null, IList<T> collection = default) : base(onItemSourceChanged, onSelectionChanged, collection ?? Array.Empty<T>()) { }
    }
}
