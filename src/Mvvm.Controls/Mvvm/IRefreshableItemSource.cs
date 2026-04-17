using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

#nullable enable

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Represents an <see cref="IRefreshableItemSource{T}"/> that is also an <see cref="ISelector{T}"/>
    /// </summary>
    public interface IRefreshableSelector<T> : IRefreshableItemSource<T>, ISelector<T>, IRefreshableSelector { }

    /// <summary>
    /// Represents an <see cref="IRefreshableItemSource"/> that is also an <see cref="ISelector"/>
    /// </summary>
    public interface IRefreshableSelector : IRefreshableItemSource, ISelector { }

    /// <inheritdoc cref="IRefreshableItemSource" />
    /// <inheritdoc cref="Primitives.ItemSource{T, E}"/>
    public interface IRefreshableItemSource<T> : IRefreshableItemSource, IItemSource<T> { }

    /// <summary>
    /// Represents an <see cref="IItemSource"/> whose collection can be refreshed.
    /// </summary>
    public interface IRefreshableItemSource : IItemSource
    {
        /// <summary>
        /// A boolean value to indicate if the collection is currently refreshing.
        /// </summary>
        bool IsRefreshing { get; }

        /// <summary>
        /// A RelayCommand that can be used to refresh the collection
        /// </summary>
        IRelayCommand RefreshCommand { get; }

        /// <summary>
        /// Gets a command that can be used to cancel the refresh if a cancellable refresh method was supplied to the constructor.
        /// </summary>
        System.Windows.Input.ICommand CancelRefreshCommand { get; }

        /// <summary>
        /// Update the ItemSource
        /// </summary>
        void Refresh();

        /// <summary>
        /// Update the ItemSource asynchronously
        /// </summary>
        Task RefreshAsync(CancellationToken token);

        /// <summary>
        /// Checks if the <see cref="IItemSource.Items"/> collection has been initialized.
        /// Will only trigger a refresh if the collection has not been initialized yet.
        /// </summary>
        /// <param name="maxWaitTime">
        /// Maximum time to wait for an asynchronous refresh to complete.
        /// If not specified, defaults to 3 seconds.
        /// </param>
        /// <exception cref="RefreshFailedException"/>
        /// <exception cref="OperationCanceledException"/>
        void EnsureInitialized(TimeSpan? maxWaitTime = null);

        /// <summary>
        /// Checks if the <see cref="IItemSource.Items"/> collection has been initialized.
        /// Will only trigger a refresh if the collection has not been initialized yet.
        /// </summary>
        /// <exception cref="RefreshFailedException"/>
        /// <exception cref="OperationCanceledException"/>
        Task EnsureInitializedAsync(CancellationToken token);

        /// <summary>
        /// Public EventHandler method to allow triggering the refresh via another object's event
        /// </summary>
        void Refresh(object? sender, EventArgs e);

        /// <summary>
        /// Public EventHandler to allow triggering the refresh via a routed event
        /// </summary>
        void Refresh(object? sender, RoutedEventArgs e);
    }    
}
