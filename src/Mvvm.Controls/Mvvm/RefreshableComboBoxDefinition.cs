using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// An <see cref="IComboBox"/> that can be refreshed.
    /// </summary>
    /// <inheritdoc cref="ListBoxDefinition{T, TList,  TSelectedValue}"/>
    public partial class RefreshableComboBoxDefinition<T, TList, TSelectedValue> : RefreshableSelector<T, TList, TSelectedValue>,
        IRefreshableItemSource, IRefreshableItemSource<T>,
        IItemSource, IItemSource<T>,
        IComboBox, IComboBox<T>
        where TList : IList<T>
    {
        /// <summary>Create a new Refreshable ComboBox </summary>
        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{CancellationToken, Task{TList}}, Func{bool}, Action, Action, bool)"/>
        /// <param name="refresh">A function to invoke that will refresh the collection</param>
        /// <param name="canRefresh">a function that is used to determine if the collection can be refreshed</param>
        /// <param name="onCollectionChanged"/><param name="onSelectionChanged"/><param name="refreshOnFirstCollectionRequest"/>
        public RefreshableComboBoxDefinition(Func<TList> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest)
        { }

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<TList> refresh) : this(refresh, null, null, null) { }

        /* Async CTORs */


        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<Task<TList>> refreshAsync, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refreshAsync, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest)
        { }

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<Task<TList>> refreshAsync) : this(refreshAsync, null, null, null) { }

        /* Cancellable CTORs */

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<CancellationToken, Task<TList>> refreshAsyncCancellable, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refreshAsyncCancellable, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest)
        { }

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<CancellationToken, Task<TList>> refreshAsyncCancellable) : this(refreshAsyncCancellable, null, null, null) { }

        /* Members */

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

    /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}"/>
    public sealed class RefreshableComboBoxDefinition<T> : RefreshableComboBoxDefinition<T, IList<T>, object>
    {
        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<IList<T>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<Task<IList<T>>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<CancellationToken, Task<IList<T>>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<IList<T>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<Task<IList<T>>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<CancellationToken, Task<IList<T>>> refresh) : base(refresh) { }
    }

    /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}"/>
    public sealed class RefreshableComboBoxDefinition<T, TSelectedValue> : RefreshableComboBoxDefinition<T, IList<T>, TSelectedValue>
    {
        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<IList<T>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<Task<IList<T>>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<CancellationToken, Task<IList<T>>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<IList<T>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<Task<IList<T>>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}.RefreshableComboBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableComboBoxDefinition(Func<CancellationToken, Task<IList<T>>> refresh) : base(refresh) { }
    }
}
