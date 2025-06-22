using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Collections;
using CommunityToolkit.Mvvm.ComponentModel;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm.Primitives
{
    /// <summary>
    /// Represents an ItemSource binding
    /// </summary>
    /// <typeparam name="T"> The type of objects within the collection </typeparam>
    /// <typeparam name="TList"> The type of collection - must implement <see cref="IEnumerable{T}"/></typeparam>
    public partial class ItemSource<T,TList> : ControlBase, IItemSource, IItemSource<T> 
        where TList : IList<T>
    {
        /// <summary>
        /// The default empty collection to return when the object is constructed
        /// </summary>
        /// <remarks>
        /// If able, prefers System.Collections.Immutable.ImmutableArray.
        /// <br/> Then attempts to use System.Array.Empty. 
        /// <br/> If neither collection type is valid, returns  <see langword="default"/>
        /// </remarks>
        public static readonly TList EmptyCollection;

        /// <summary>
        /// TRUE if the type of IList implements <see cref="INotifyCollectionChanged"/>, otherwise false
        /// </summary>
        public static readonly bool IsINotifyPropertyChanged;

        /// <summary>
        /// Set the default display member path for ItemSources of the specied type
        /// </summary>
        public static string DefaultDisplayMemberPath { get => _DefaultDisplayMemberPath; set => _DefaultDisplayMemberPath = value.Trim() ?? string.Empty; }
        private static string _DefaultDisplayMemberPath;

        static ItemSource()
        {
            IsINotifyPropertyChanged = typeof(TList) is INotifyCollectionChanged;
            _DefaultDisplayMemberPath = string.Empty;

#if NET5_0_OR_GREATER
            if (System.Collections.Immutable.ImmutableArray<T>.Empty is TList immuatableEmpty)
            {
                EmptyCollection = immuatableEmpty;
                return;
            }
#endif
            if (Array.Empty<T>() is TList emptyArray)
            {
                EmptyCollection = emptyArray;
                return;
            }
            EmptyCollection = default;
        }

        /// <summary>
        /// Initialize the ItemSource
        /// </summary>
        public ItemSource() { }

        /// <summary>
        /// Initialize the ItemSource
        /// </summary>
        public ItemSource(TList collection) : this(null, collection) { }

        /// <summary>
        /// Create a new selector
        /// </summary>
        /// <param name="collection">A collection of items. Default or null is OK.</param>
        /// <param name="onCollectionChanged">An action to be inoked when the collection changes (such as calling IRelayCommand.CanExecuteChanged)</param>
        public ItemSource(Action onCollectionChanged = null, TList collection = default)
        {
            Items = collection ?? EmptyCollection;
            _onCollectionChanged = onCollectionChanged;
        }

        private readonly Action? _onCollectionChanged;
        private TList _items = EmptyCollection;

        /// <summary>
        /// Occurs after the item source has been updated. For most implementations, this will mean the object has been replaced. 
        /// </summary>
        /// <remarks>If the collection type implements INotifyCollectionChanged, this event will fire when the collection changes, and the event args will be <see cref="NotifyCollectionChangedEventArgs"/></remarks>
        public event EventHandler ItemSourceChanged;

        /// <summary>
        /// Set the DisplayMemberPath of the control
        /// </summary>
        /// <remarks>
        /// Object Person(Joe,Schmoe), DisplayMemberPath = 'FirstName', Display will be 'Joe'
        /// </remarks>
        [ObservableProperty]
        private string _displayMemberPath = DefaultDisplayMemberPath;

        /// <summary>
        /// Binding for <see cref="ItemsControl.ItemsSource"/>
        /// </summary>
        /// Can NOT be a [ObservableProperty] because it may need to be overriden
        public virtual TList Items
        {
            get { return _items; }
            set 
            {
                if (!EqualityComparer<TList>.Default.Equals(_items, value))
                {
                    OnPropertyChanging(EventArgSingletons.Items);

                    // unsubscribe from the old
                    if (_items is INotifyCollectionChanged coll)
                    {
                        coll.CollectionChanged -= RaiseItemSourceChanged;
                    }

                    OnItemsChanging();

                    _items = value;

                    // subscribe to the new
                    if (value is INotifyCollectionChanged newColl)
                    {
                        newColl.CollectionChanged += RaiseItemSourceChanged;

                    }

                    // standard notifications
                    OnItemSourceChanged(EventArgs.Empty);
                    OnPropertyChanged(EventArgSingletons.Items);

                    // invoke derived class requirements
                    OnItemsChanged();

                    // Invoke the optional action  passed to the constructor 
                    _onCollectionChanged?.Invoke();
                }
            }
        }

        IList IItemSource.Items { get => (IList)Items; }
        IList<T> IItemSource<T>.Items { get => Items; }

        /// <summary> method called after raising the <see cref="System.ComponentModel.INotifyPropertyChanging.PropertyChanging"/> event, but before the change takes place. </summary>
        /// <remarks>Access the current collection via <see cref="Items"/></remarks>
        protected virtual void OnItemsChanging() { }

        /// <summary> method called after the collection has been updated, but before the <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/> event is raised. </summary>
        /// <remarks>Access the current collection via <see cref="Items"/></remarks>
        protected virtual void OnItemsChanged() { }

        private void RaiseItemSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnItemSourceChanged(e);
        }

        /// <summary> Raises the SelectionChanged event </summary>
        protected virtual void OnItemSourceChanged(EventArgs e)
        {
            ItemSourceChanged?.Invoke(this, e);
        }
    }
}
