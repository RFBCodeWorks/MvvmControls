using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

#nullable enable

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
        ISelector, 
        ISelector<T>,
        IRefreshableItemSource,
        IRefreshableItemSource<T>,
        IRefreshableSelector,
        IRefreshableSelector<T>
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
        public RefreshableSelector(Func<TList> refresh, Func<bool>? canRefresh = null, Action? onCollectionChanged = null, Action? onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(onCollectionChanged, onSelectionChanged, EmptyCollection)
        {
            _refresh = refresh;
            _canRefresh = canRefresh ?? ReturnTrue;
            itemsInitialized = SetRequiresInitialization(refreshOnFirstCollectionRequest);
        }

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<TList> refresh) : this(refresh, null, null, null) { }

#pragma warning restore CS1572

        /* Async CTORs */

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<Task<TList>> refreshAsync, Func<bool>? canRefresh = null, Action? onCollectionChanged = null, Action? onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(onCollectionChanged, onSelectionChanged, EmptyCollection)
        {
            _cancellableRefreshAsync = (c) => refreshAsync();
            _cancelRefreshCommand = InactiveButton.Instance;
            _canRefresh = canRefresh ?? ReturnTrue;
            itemsInitialized = SetRequiresInitialization(refreshOnFirstCollectionRequest);
        }

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<Task<TList>> refreshAsync) : this(refreshAsync, null, null, null) { }

        /* Cancellable CTORs */

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<CancellationToken, Task<TList>> refreshAsyncCancellable, Func<bool>? canRefresh = null, Action? onCollectionChanged = null, Action? onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(onCollectionChanged, onSelectionChanged, EmptyCollection)
        {
            _cancellableRefreshAsync = refreshAsyncCancellable;
            _canRefresh = canRefresh ?? ReturnTrue;
            itemsInitialized = SetRequiresInitialization(refreshOnFirstCollectionRequest);
        }

        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableSelector(Func<CancellationToken, Task<TList>> refreshAsyncCancellable) : this(refreshAsyncCancellable, null, null, null) { }

        /* Members  */

        private IRelayCommand? _refreshCommand;
        private ICommand? _cancelRefreshCommand;
        private int itemsInitialized;
        private int _isRefreshing;
        private readonly Func<bool> _canRefresh;
        private readonly Func<TList>? _refresh;
        private readonly Func<CancellationToken, Task<TList>>? _cancellableRefreshAsync;
        private Task? _isRefreshingTcs;

        /// <inheritdoc/>
        public sealed override TList Items
        {
            get
            {
                if (itemsInitialized.CompareExchange(1, -1))
                {
                    RefreshCommand.Execute(null);
                }
                return base.Items;
            }
            set
            {
                base.Items = value;
                itemsInitialized = 1;
            }
        }

        /// <summary>
        /// Set <see langword="true"/> when the refresh command is invoked and set <see langword="false"/> after the collection has been updated.
        /// </summary>
        public bool IsRefreshing
        {
            get => _isRefreshing == 1;
            private set
            {
                int newValue = value ? 1 : 0;
                if (_isRefreshing != newValue)
                {
                    OnPropertyChanging(EventArgSingletons.IsRefreshing);
                    _isRefreshing = newValue;
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
                return new RelayCommand(RefreshAction, _canRefresh);
            }
            else if (_cancellableRefreshAsync is not null)
            {
                return new AsyncRelayCommand(RefreshTask, _canRefresh, AsyncRelayCommandOptions.None);
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

            // check if already refreshing -- if _isRefreshing = 0, set to 1 and return 0
            if (Interlocked.CompareExchange(ref _isRefreshing, 1, 0) == 1 && _isRefreshingTcs is not null)
            {
                return _isRefreshingTcs;
            }

            // this thread starts the refresh
            OnPropertyChanging(EventArgSingletons.IsRefreshing);
            var task =  DoRefreshTask(token);
            _isRefreshingTcs = task;
            OnPropertyChanged(EventArgSingletons.IsRefreshing);
            return task;
        }

        private async Task DoRefreshTask(CancellationToken token)
        {
            try
            {
                if (_cancellableRefreshAsync is not null)
                    Items = await _cancellableRefreshAsync(token).ConfigureAwait(true);
            }
            finally
            {
                Interlocked.Exchange(ref _isRefreshing, 0);
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        private void ResetInitializedState() => Interlocked.Exchange(ref itemsInitialized, 0);
        private bool RequiresInitialization() => IsRefreshing == false && Interlocked.Exchange(ref itemsInitialized, 1) < 1;
        private static int SetRequiresInitialization(bool refreshOnFirstCollectionRequest) => refreshOnFirstCollectionRequest ? -1 : 0;

#if NET8_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
#endif
        private static void ThrowRefreshFailedException(string message = "Collection initialization failed", Exception? innerException = null)
            => throw new RefreshFailedException(message, innerException);

        /// <summary>
        /// Checks if the <see cref="Items"/> collection has been initialized.
        /// <br/> Will only trigger a refresh if the collection has not been initialized yet.
        /// </summary>
        /// <param name="maxWaitTime">
        /// <see langword="Note:"/>  
        /// <br/> - This parameter will only take effect for asynchronous refreshes that respect cancellation, or if the refresh was already triggered from another thread.
        /// <br/> - <see cref="EnsureInitializedAsync(CancellationToken)"/> should be preferred where possible, especially from a UI thread.
        /// <para/> Maximum time delay to block the thread while waiting for an asynchronous refresh to complete.
        /// <br/> If not specified, defaults to 3 seconds.
        /// </param>
        /// <exception cref="RefreshFailedException" />
        /// <exception cref="OperationCanceledException" />
        public void EnsureInitialized(TimeSpan? maxWaitTime = null)
        {
            bool usingToken = false;
            try
            {
                if (RequiresInitialization())
                {
                    if (RefreshCommand.CanExecute(null) == false)
                    {
                        ThrowRefreshFailedException("Unable to initialize collection: RefreshCommand.CanExecute() returned false");
                    }
                    if (RefreshCommand is IAsyncRelayCommand)
                    {
                        usingToken = true;
                        using CancellationTokenSource waitTime = new(maxWaitTime ?? TimeSpan.FromMilliseconds(3000));
                        RefreshAsync(waitTime.Token).Wait(waitTime.Token);
                    }
                    else
                    {
                        RefreshCommand.Execute(null);
                    }
                }
                else if (IsRefreshing)
                {
                    usingToken = true;
                    using CancellationTokenSource waitTime = new(maxWaitTime ?? TimeSpan.FromMilliseconds(3000));
                    SpinWait.SpinUntil(() => waitTime.IsCancellationRequested || IsRefreshing == false);
                    
                    if (IsRefreshing) 
                        waitTime.Token.ThrowIfCancellationRequested();
                }
            }
            catch (RefreshFailedException) { ResetInitializedState(); throw; }
            catch (OperationCanceledException e) when (usingToken)
            {
                ResetInitializedState();
                ThrowRefreshFailedException("Exceeded maximum wait time for collection initialization.", e);
            }
            catch (Exception e)
            {
                ResetInitializedState();
                ThrowRefreshFailedException(innerException: e);
            }
            if (Items is null)
            {
                ResetInitializedState();
                ThrowRefreshFailedException("Collection initialization failed: Items collection is null.");
            }
        }

        /// <summary>
        /// Checks if the collection has been initialized. 
        /// <br/> Will only trigger a refresh if the collection has not been initialized yet.
        /// </summary>
        /// <exception cref="RefreshFailedException" />
        /// <exception cref="OperationCanceledException" />
        public async Task EnsureInitializedAsync(CancellationToken token)
        {
            try
            {
                if (RequiresInitialization())
                {
                    if (RefreshCommand.CanExecute(null) == false)
                    {
                        ThrowRefreshFailedException("Unable to initialize collection: RefreshCommand.CanExecute() returned false");
                    }
                    await RefreshAsync(token);
                }

                else if (RefreshCommand is IAsyncRelayCommand asyncCommand && asyncCommand.ExecutionTask is not null)
                {
                    var task = asyncCommand.ExecutionTask;
                    if (task.Status < TaskStatus.RanToCompletion)
                    {
                        var tcs = new TaskCompletionSource<object>();
                        using var reg = token.Register(() => tcs.SetCanceled());
                        var completedTask = await Task.WhenAny(tcs.Task, task); // returns when cancellation is requested or the refresh task completes
                        if (completedTask == tcs.Task)
                        {
                            token.ThrowIfCancellationRequested();
                        }
                    }
                    await task;
                }

                else if (IsRefreshing)
                {
                    // handle synchronous refresh from another thread
                    while (IsRefreshing)
                    {
                        await Task.Delay(50, token);
                    }
                }
            }
            catch (OperationCanceledException) { ResetInitializedState(); throw; }
            catch (RefreshFailedException) { ResetInitializedState(); throw; }
            catch (Exception e)
            {
                ResetInitializedState();
                ThrowRefreshFailedException(innerException: e);
            }
            if (Items is null)
            {
                ResetInitializedState();
                ThrowRefreshFailedException("Collection initialization failed: Items collection is null.");
            }
        }

        

        /// <inheritdoc/>
        public void Refresh(object? sender, EventArgs e) => RefreshCommand.Execute(null);
        
        /// <inheritdoc/>
        public void Refresh(object? sender, RoutedEventArgs e) => RefreshCommand.Execute(null);

        /// <inheritdoc/>
        /// <remarks>Invokes the <see cref="RefreshCommand"/>'s execute method</remarks>
        public void Refresh()
        {
            if (RefreshCommand is IAsyncRelayCommand)
            {
                RefreshAsync(CancellationToken.None).Wait();
            }
            else if (RefreshCommand.CanExecute(null))
            {
                RefreshCommand.Execute(null);
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// If currently refreshing, returns the underlying task.
        /// </remarks>
        public Task RefreshAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (RefreshCommand is IAsyncRelayCommand asyncCmd)
            {
                using var cancellationReg = token.Register(asyncCmd.Cancel);
                if (asyncCmd.IsRunning)
                {
                    return asyncCmd.ExecutionTask!;
                }
                return asyncCmd.ExecuteAsync(token);
            }
            else if (_refresh is not null)
            {
                if (IsRefreshing)
                {
                    return _isRefreshingTcs!;
                }
                if (_canRefresh())
                {
                    return _isRefreshingTcs = Task.Run(RefreshAction, token);
                }
            }
            return Task.CompletedTask;
        }
    }
}
