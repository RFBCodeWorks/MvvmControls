using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// An <see cref="ISelector"/> that allows refreshing.
    /// <para/>
    /// Base class for :
    /// <br/><see cref="RefreshableComboBoxDefinition{T, TList, TSelectedValue}"/>
    /// <br/><see cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TList"></typeparam>
    /// <typeparam name="TSelectedValue"></typeparam>
    public class RefreshableSelector<T, TList, TSelectedValue> : Primitives.SelectorDefinition<T, TList, TSelectedValue>, 
        ICommand,
        IRelayCommand,
        ISelector, 
        ISelector<T>,
        IRefreshableItemSource,
        IRefreshableItemSource<T>
        where TList : IList<T>
    {

#pragma warning disable CS1572 // XML comment has a param tag, but there is no parameter by that name { Provides parameter definition for overloads }

        /// <summary>Create a new RefreshableComboBox </summary>
        /// <param name="refresh">A function to invoke that will refresh the collection</param>
        /// <param name="refreshAsync">An async function that will refresh the collection</param>
        /// <param name="refreshAsyncCancellable">A cancellable async function to that will refresh the collection</param>
        /// <param name="canRefresh">a function that is used to determine if the collection can be refreshed</param>
        /// <param name="refreshOnFirstCollectionRequest">
        ///     When <see langword="true"/> (default) : refreshes the collection the first time the <see cref="Items"/> property is retrieved.
        ///     <br/>Note that if the refresh function is Task{TList} this may occur asynchronously.
        ///     <para/>When <see langword="false"/> : does not refresh until RefreshCommand\Refresh\ResfreshAsync is invoked.
        ///     <para/>Setting the Items collection via the property will also disable the first refresh.
        /// </param>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        /// <param name="onCollectionChanged"/><param name="onSelectionChanged"/>
        public RefreshableSelector(Func<TList> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(onCollectionChanged, onSelectionChanged, EmptyCollection)
        {
            _refresh = refresh;
            _canRefresh = canRefresh;
            itemsInitialized = !refreshOnFirstCollectionRequest;
        }

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<TList> refresh) : this(refresh, null, null, null) { }

#pragma warning restore CS1572

        /* Async CTORs */

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<Task<TList>> refreshAsync, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(onCollectionChanged, onSelectionChanged, EmptyCollection)
        {
            _refreshAsync = refreshAsync;
            _canRefresh = canRefresh;
            itemsInitialized = !refreshOnFirstCollectionRequest;
        }

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<Task<TList>> refreshAsync) : this(refreshAsync, null, null, null) { }

        /* Cancellable CTORs */

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<CancellationToken, Task<TList>> refreshAsyncCancellable, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(onCollectionChanged, onSelectionChanged, EmptyCollection)
        {
            _cancellableRefreshAsync = refreshAsyncCancellable;
            _canRefresh = canRefresh;
            itemsInitialized = !refreshOnFirstCollectionRequest;
        }

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<CancellationToken, Task<TList>> refreshAsyncCancellable) : this(refreshAsyncCancellable, null, null, null) { }

        /* Members  */

        event EventHandler ICommand.CanExecuteChanged
        {
            add 
            {
                if (RefreshCommand is not InactiveButton)
                {
                    RefreshCommand.CanExecuteChanged += value;
                }
            }
            remove
            {
                RefreshCommand.CanExecuteChanged -= value;
            }
        }

        private IRelayCommand _refreshCommand;
        private ICommand _cancelRefreshCommand;
        private bool itemsInitialized;
        private int _isRefreshing;
        private readonly Func<bool> _canRefresh;
        private readonly Func<TList> _refresh;
        private readonly Func<Task<TList>> _refreshAsync;
        private readonly Func<CancellationToken, Task<TList>> _cancellableRefreshAsync;
        private Task _isRefreshingTcs;

        /// <inheritdoc/>
        public sealed override TList Items
        {
            get
            {
                if (!itemsInitialized)
                {
                    itemsInitialized = true;
                    RefreshCommand.Execute(null);
                }
                return base.Items;
            }
            set
            {
                base.Items = value;
                itemsInitialized = true;
            }
        }

        /// <summary>
        /// Set <see langword="true"/> when the refresh command is invoked and set <see langword="false"/> after the collection has been updated.
        /// </summary>
        public bool IsRefreshing
        {
            get => _isRefreshing == 0;
            private set
            {
                if (value != (_isRefreshing == 0))
                {
                    OnPropertyChanging(EventArgSingletons.IsRefreshing);
                    _isRefreshing = value ? 1 : 0;
                    OnPropertyChanged(EventArgSingletons.IsRefreshing);
                }
            }
        }

        /// <inheritdoc/>
        public IRelayCommand RefreshCommand => _refreshCommand ??= CreateCommand();

        /// <inheritdoc/>
        public ICommand CancelRefreshCommand => _cancelRefreshCommand ??= GetCancelCommand();

        private IRelayCommand CreateCommand()
        {
            if (_refresh is not null)
            {
                return new RelayCommand(RefreshAction, _canRefresh ?? ReturnTrue);
            }
            else if (_refreshAsync is not null || _cancellableRefreshAsync is not null)
            {
                return new AsyncRelayCommand(RefreshTask, _canRefresh ?? ReturnTrue, AsyncRelayCommandOptions.None);
            }
            return InactiveButton.Instance;
        }
        private ICommand GetCancelCommand()
        {
            if (_cancellableRefreshAsync is null)
                return InactiveButton.Instance;
            else
                return IAsyncRelayCommandExtensions.CreateCancelCommand((IAsyncRelayCommand)RefreshCommand);
        }

        /// <summary> The action too provide a RelayCommand </summary>
        private void RefreshAction()
        {
            if (_refresh is null) return;
            try
            {
                IsRefreshing = true;
                Items = _refresh();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary> This is the task to provide an AsyncRelayCommand </summary>
        private Task RefreshTask(CancellationToken token)
        {
            if (token.IsCancellationRequested) return Task.FromCanceled(token);

            // check if already refreshing
            if (Interlocked.CompareExchange(ref _isRefreshing, 1, 0) == 1)
            {
                return _isRefreshingTcs;
            }

            // this thread starts the refresh
            _isRefreshingTcs = DoRefreshTask(token);
            OnPropertyChanged(nameof(IsRefreshing));
            return _isRefreshingTcs;
        }

        private async Task DoRefreshTask(CancellationToken token)
        {
            try
            {
                if (_refreshAsync is not null)
                    Items = await _refreshAsync().ConfigureAwait(true);

                else if (_cancellableRefreshAsync is not null)
                    Items = await _cancellableRefreshAsync(token).ConfigureAwait(true);
            }
            finally
            {
                Interlocked.Exchange(ref _isRefreshing, 0);
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
        

        /// <inheritdoc/>
        public void Refresh(object sender, EventArgs e) => RefreshCommand.Execute(null);
        /// <inheritdoc/>
        public void Refresh(object sender, RoutedEventArgs e) => RefreshCommand.Execute(null);

        /// <inheritdoc/>
        /// <remarks>Invokes the <see cref="RefreshCommand"/>'s execute method</remarks>
        public void Refresh()
        {
            if (RefreshCommand.CanExecute(null))
            {
                if (RefreshCommand is AsyncRelayCommand asyncCmd)
                {
                    asyncCmd.ExecuteAsync(null).Wait();
                }
                else
                {
                    RefreshCommand.Execute(null);
                }
            }
        }

        /// <inheritdoc/>
        public async Task RefreshAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (RefreshCommand is AsyncRelayCommand asyncCmd)
            {
                // need to create a cancellation event here
                if (token.CanBeCanceled)
                {
                    var cancellation = token.Register(() => asyncCmd.Cancel());
                    await asyncCmd.ExecuteAsync(token);
                    cancellation.Dispose();
                }
                else
                {
                    await _cancellableRefreshAsync(token);
                }
            }
            else if (_refresh is not null && (_canRefresh??ReturnTrue).Invoke())
            {
                await Task.Run(RefreshAction, token);
            }
        }

        /// <summary>
        /// Notify the <see cref="RefreshCommand"/>
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void NotifyCanExecuteChanged() => _refreshCommand?.NotifyCanExecuteChanged();
        bool ICommand.CanExecute(object parameter) => _refreshCommand?.CanExecute(parameter) ?? false;
        void ICommand.Execute(object parameter) => _refreshCommand?.Execute(parameter);
    }
}
