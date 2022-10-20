using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RFBCodeWorks.MVVMObjects.BaseControlDefinitions;
using RFBCodeWorks.MVVMObjects.ControlInterfaces;

namespace RFBCodeWorks.MVVMObjects.ControlInterfaces
{
    /// <summary>
    /// Interface all ComboBoxDefinitions should implement to be assignable via AttachedProperty
    /// </summary>
    public interface IComboBoxDefinition : ISelector
    {
        /// <inheritdoc cref="System.Windows.Controls.ComboBox.IsDropDownOpen"/>
        /// <remarks>This property can be used to force the drop-down open from the ViewModel</remarks>
        public bool IsDropDownOpen { get; set; }
    }

    /// <summary>
    /// Interface all ComboBoxDefinitions of some type implement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IComboBoxDefinition<T> : ISelector<T>, IComboBoxDefinition { }
}

namespace RFBCodeWorks.MVVMObjects
{

    /// <summary>
    /// The base ComboBoxDefinition
    /// </summary>
    /// <inheritdoc cref="SelectorDefinition{T, E, V}"/>
    public class ComboBoxDefinition<T, E, V> : SelectorDefinition<T, E, V>, IComboBoxDefinition, IComboBoxDefinition<T>
        where E : IList<T>
    {
        /// <inheritdoc cref="System.Windows.Controls.ComboBox.Text"/>
        /// <remarks>
        /// When the IsEditable property is true, setting this property places initial text entered in the text box. When IsEditable is false, setting this value has no effect.
        /// <br/>
        /// See the link below for more information on this interaction: <br/>
        /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox?view=windowsdesktop-6.0"/>
        /// </remarks>
        public string Text { get; set; }

        /// <inheritdoc cref="System.Windows.Controls.ComboBox.IsEditable"/>
        /// <remarks>
        /// See the link below for more information on this interaction: <br/>
        /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox?view=windowsdesktop-6.0"/>
        /// </remarks>
        public bool IsEditable { get; set; } = false;

        /// <returns>
        /// true if the ComboBox is read-only; otherwise, false. <br/>
        /// The default for the control is false, the default for this definition is true.
        /// </returns>
        /// <inheritdoc cref="System.Windows.Controls.ComboBox.IsReadOnly"/>
        /// <remarks>
        /// See the link below for more information on this interaction: <br/>
        /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox?view=windowsdesktop-6.0"/>
        /// </remarks>
        public bool IsReadOnly { get; set; } = true;

        /// <inheritdoc/>
        public bool IsDropDownOpen
        {
            get { return IsDropDownOpenField; }
            set { SetProperty(ref IsDropDownOpenField, value, nameof(IsDropDownOpen)); }
        }
        private bool IsDropDownOpenField;

    }

    #region < ComboBox Definition >

    /// <summary>
    /// A Simple ComboBox Definition that only specifies the enumerated type
    /// </summary>
    /// <inheritdoc cref="ComboBoxDefinition{T, E, V}"/>
    public class ComboBoxDefinition<T> : ComboBoxDefinition<T, IList<T>, object> { }

    /// <summary>
    /// A generic bindable definition for a ComboBox whose ItemSource is any IEnumerable object, that expects a SelectedValue of a specific type
    /// </summary>
    /// <inheritdoc cref="ComboBoxDefinition{T, E, V}"/>
    public class ComboBoxDefinition<T, V> : ComboBoxDefinition<T, IList<T>, V> { }

    /// <summary>
    /// A definition for a combox where the ItemSource is a specific type of <see cref="IList{T}"/>
    /// </summary>
    /// <inheritdoc cref="ComboBoxDefinition{T, E, V}"/>
    public class ComboBoxDefinition2<T, E> : ComboBoxDefinition<T, E, object> where E : IList<T> { }

    #endregion'

    #region < Refreshable >

    /// <inheritdoc cref="RefreshableComboBoxDefinition{T, V}"/>
    public class RefreshableComboBoxDefinition<T> : RefreshableComboBoxDefinition<T, object> { }

    /// <summary>
    /// A ComboBoxDefinition whose collection is an array of <typeparamref name="T"/> objects, that can be refreshed on demand via the <see cref="RefreshFunc"/>
    /// </summary>
    /// <inheritdoc cref="ComboBoxDefinition{T, E, V}"/>
    public class RefreshableComboBoxDefinition<T, V> : ComboBoxDefinition<T, T[], V>, IRefreshableItemSource<T>
    {
        /// <summary>
        /// Create a new RefreshableComboBox object
        /// </summary>
        public RefreshableComboBoxDefinition()
        {
            RefreshCommand = new RelayCommand(Refresh, () => CanRefresh());
        }

        /// <inheritdoc/>
        public Func<T[]> RefreshFunc { get; init; }
        
        /// <inheritdoc/>
        public Func<bool> CanRefresh { get; init; }
        
        /// <inheritdoc/>
        public IButtonDefinition RefreshCommand { get; }

        /// <summary>
        /// Check if the <see cref="RefreshFunc"/> is null and return the opposite
        /// </summary>
        /// <returns>TRUE if not null, FALSE is null</returns>
        protected bool HasRefreshFunc()
        {
            return RefreshFunc != null;
        }

        /// <inheritdoc/>
        public virtual void Refresh()
        {
            if (RefreshFunc != null && (CanRefresh?.Invoke() ?? true))
                ItemSource = RefreshFunc();
        }

        /// <inheritdoc/>
        public void Refresh(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <inheritdoc/>
        public void Refresh(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }

    #endregion

    #region < Concurrent >

    /// <inheritdoc cref="ConcurrentComboBoxDefinition{T, V}"/>
    public class ConcurrentComboBoxDefinition<T> : ConcurrentComboBoxDefinition<T, object> { }

    /// <summary>
    /// A ComboBox that uses a thread-safe list to update its contents
    /// </summary>
    /// <inheritdoc cref="ComboBoxDefinition{T, E, V}"/>
    public class ConcurrentComboBoxDefinition<T, V> : ComboBoxDefinition<T, ConcurrentObservableCollection<T>, V>, IConcurrentItemSource<T>
    {
        /// <summary>
        /// Initialize the definition
        /// </summary>
        public ConcurrentComboBoxDefinition()
        {
            ItemSource = new();
        }

        /// <inheritdoc/>
        public new ConcurrentObservableCollection<T> ItemSource { get => base.ItemSource; init => base.ItemSource = value; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void Add(T item)
        {
            ItemSource.Add(item);
        }

        public void AddRange(params T[] items)
        {
            ItemSource.AddRange(items);
        }

        public void AddRange(IEnumerable<T> items)
        {
            ItemSource.AddRange(items);
        }

        public void Clear()
        {
            ItemSource.Clear();
        }

        public void Remove(T item)
        {
            ItemSource.Remove(item);
        }

        public void RemoveAt(int index)
        {
            ItemSource.RemoveAt(index);
        }

        public void RemoveAll(params T[] items)
        {
            ItemSource.RemoveAll(items);
        }

        public void RemoveAll(Predicate<T> match)
        {
            ItemSource.RemoveAll(match);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }


    #endregion
}
