using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
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

        private static async Task WaitForTaskOrCancellationAsync(Task task, CancellationToken token)
        {
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
        }

        private async Task AwaitLatestExecutionTaskAsync(IAsyncRelayCommand asyncCommand, CancellationToken token)
        {
            while (asyncCommand.ExecutionTask is Task task)
            {
                await WaitForTaskOrCancellationAsync(task, token);

                try
                {
                    await task;
                    return;
                }
                catch (OperationCanceledException) when (!token.IsCancellationRequested)
                {
                    var currentTask = asyncCommand.ExecutionTask;
                    if (ReferenceEquals(currentTask, task) && !asyncCommand.IsRunning && !IsRefreshing)
                    {
                        await asyncCommand.ExecuteAsync(token);
                        return;
                    }

                    // Another refresh has superseded this one (or is still in-flight).
                    // Continue waiting for the latest execution task.
                }
            }
        }

        private async Task RefreshAsyncInternal(IAsyncRelayCommand asyncCmd, CancellationToken token)
        {
            using var cancellationReg = token.Register(asyncCmd.Cancel);
            if (asyncCmd.IsRunning)
            {
                await AwaitLatestExecutionTaskAsync(asyncCmd, token);
            }
            else
            {
                await asyncCmd.ExecuteAsync(token);
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
                return new AsyncRelayCommand(RefreshTask, _canRefresh, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
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
            var tcs = new TaskCompletionSource<bool>();
            _isRefreshingTcs = tcs.Task;
            try
            {
                IsRefreshing = true;
                var result = _refresh();
                tcs.SetResult(true);
                Items = result;
            }
            catch(Exception e)
            {
                tcs.SetException(e);
                throw;
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
                OnPropertyChanging(EventArgSingletons.IsRefreshing);
                Interlocked.Exchange(ref _isRefreshing, 0);
                OnPropertyChanged(EventArgSingletons.IsRefreshing);
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
            // Call the specialized method for asynchronous waiting
            if (RefreshCommand is IAsyncRelayCommand)
            {
                using CancellationTokenSource waitTime = new(maxWaitTime ?? TimeSpan.FromMilliseconds(3000));
                try
                {
                    EnsureInitializedAsync(waitTime.Token).Wait(waitTime.Token);
                    return;
                }
                catch (OperationCanceledException e) when (waitTime.IsCancellationRequested)
                {
                    ThrowRefreshFailedException("Exceeded maximum wait time for asynchronous collection initialization.", e);
                }
                catch (AggregateException eWaitForTask)
                {
                    // Is this just a wrapped exception?
                    // https://gist.github.com/tintoy/0763a1f53ee62510e681a05f46772849
                    AggregateException flattenedAggregate = eWaitForTask.Flatten();
                    if (flattenedAggregate.InnerExceptions.Count != 1)
                        throw; // Nope, genuine aggregate.

                    // Yep, so rethrow (preserving original stack-trace).
                    ThrowRefreshFailedException($"Collection Initialization failed: {flattenedAggregate.InnerExceptions[0].Message}", flattenedAggregate.InnerExceptions[0]);
                }
            }

            // synchronous command logic
            bool usingToken = false;
            try
            {
                if (RequiresInitialization())
                {
                    if (RefreshCommand.CanExecute(null) == false)
                    {
                        ThrowRefreshFailedException("Unable to initialize collection: RefreshCommand.CanExecute() returned false");
                    }
                    else
                    {
                        RefreshCommand.Execute(null);
                    }
                }
                else if (IsRefreshing && _isRefreshingTcs is not null)
                {
                    usingToken = true;
                    using CancellationTokenSource waitTime = new(maxWaitTime ?? TimeSpan.FromMilliseconds(3000));
                    _isRefreshingTcs.Wait(waitTime.Token);
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
                    await AwaitLatestExecutionTaskAsync(asyncCommand, token);
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
            catch (OperationCanceledException) when (!token.IsCancellationRequested && RefreshCommand is IAsyncRelayCommand)
            {
                if (RefreshCommand is IAsyncRelayCommand asyncCommand)
                {
                    try
                    {
                        await AwaitLatestExecutionTaskAsync(asyncCommand, token);
                    }
                    catch (OperationCanceledException) when (!token.IsCancellationRequested)
                    {
                        ResetInitializedState();
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
                return RefreshAsyncInternal(asyncCmd, token);
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
