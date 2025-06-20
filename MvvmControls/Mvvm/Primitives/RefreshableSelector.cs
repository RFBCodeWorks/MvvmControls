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

namespace RFBCodeWorks.Mvvm.Primitives
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
    public class RefreshableSelector<T, TList, TSelectedValue> : SelectorDefinition<T, TList, TSelectedValue>, 
        ICommand,
        IRelayCommand,
        ISelector, 
        ISelector<T>,
        IRefreshableItemSource,
        IRefreshableItemSource<T>
        where TList : IList<T>
    {
        /// <summary>Create a new RefreshableComboBox </summary>
        /// <param name="refresh">A function to invoke that will refresh the collection</param>
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
        public RefreshableSelector(Func<Task<TList>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(onCollectionChanged, onSelectionChanged, EmptyCollection)
        {
            _refreshAsync = refresh;
            _canRefresh = canRefresh;
            itemsInitialized = !refreshOnFirstCollectionRequest;
        }

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<CancellationToken, Task<TList>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(onCollectionChanged, onSelectionChanged, EmptyCollection)
        {
            _cancellableRefreshAsync = refresh;
            _canRefresh = canRefresh;
            itemsInitialized = !refreshOnFirstCollectionRequest;
        }

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<TList> refresh) : this(refresh, null, null, null) { }

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<Task<TList>> refresh) : this(refresh, null, null, null) { }

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<CancellationToken, Task<TList>> refresh) : this(refresh, null, null, null) { }

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
        private readonly Func<bool> _canRefresh;
        private readonly Func<TList> _refresh;
        private readonly Func<Task<TList>> _refreshAsync;
        private readonly Func<CancellationToken, Task<TList>> _cancellableRefreshAsync;

        /// <inheritdoc/>
        public sealed override TList Items
        {
            get
            {
                if (base.Items is null && !itemsInitialized)
                {
                    RefreshCommand.Execute(Items);
                    itemsInitialized = true;
                }
                return base.Items;
            }
            set
            {
                base.Items = value;
                itemsInitialized = true;
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
                return new RelayCommand(() => Items = _refresh(), _canRefresh);
            }
            else if (_refreshAsync is not null || _cancellableRefreshAsync is not null)
            {
                return new AsyncRelayCommand(RefreshTask, _canRefresh);
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

        private async Task RefreshTask(CancellationToken token)
        {
            if (token.IsCancellationRequested) return;
            try
            {
                if (_refreshAsync is not null)
                    Items = await _refreshAsync().ConfigureAwait(true);

                else if (_cancellableRefreshAsync is not null)
                    Items = await _cancellableRefreshAsync(token).ConfigureAwait(true);
            }
            catch (OperationCanceledException) { }
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
            else if (_refreshAsync is not null)
            {
                Items = await _refreshAsync();
            }
            else if (_refresh is not null)
            {
                await Task.Run(() => Items = _refresh(), token);
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
