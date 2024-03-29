﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Collections;

namespace RFBCodeWorks.Mvvm.Primitives
{
    /// <summary>
    /// Represents an ItemSource binding
    /// </summary>
    /// <typeparam name="T"> The type of objects within the collection </typeparam>
    /// <typeparam name="E"> The type of collection - must implement <see cref="IEnumerable{T}"/></typeparam>
    public class ItemSource<T,E> : ControlBase, IItemSource, IItemSource<T,E> 
        where E : IList<T>
    {

        #region < Constructors >

        /// <summary>
        /// Initialize the ItemSource
        /// </summary>
        public ItemSource() { }


        static ItemSource()
        {
            IsINotifyPropertyChanged = typeof(E) is INotifyCollectionChanged;
        }

        /// <summary>
        /// TRUE if the type of IList implements <see cref="INotifyCollectionChanged"/>, otherwise false
        /// </summary>
        public static bool IsINotifyPropertyChanged { get; }

        /// <summary>
        /// Set the default display member path for ItemSources of the specied type
        /// </summary>
        public static string DefaultDisplayMemberPath { get; set; } = "";

        #endregion

        #region < Events >

        /// <summary>
        /// Occurs after the item source has been updated. For most implementations, this will mean the object has been replaced. 
        /// </summary>
        /// <remarks>If the collection type implements INotifyCollectionChanged, this event will fire when the collection changes, and the event args will be <see cref="NotifyCollectionChangedEventArgs"/></remarks>
        public event EventHandler ItemSourceChanged;

        /// <summary> Raises the OnItemSourceChanged event - Does not differentiate between old and new items </summary>
        protected void OnItemSourceChanged()
        {
            OnItemSourceChanged(new());
        }

        /// <summary> Raises the SelectionChanged event </summary>
        protected virtual void OnItemSourceChanged(EventArgs e)
        {
            ItemSourceChanged?.Invoke(this, e);
        }

        #endregion

        #region < Properties >

        /// <summary>
        /// Set the DisplayMemberPath of the control
        /// </summary>
        /// <remarks>
        /// Object Person(Joe,Schmoe), DisplayMemberPath = 'FirstName', Display will be 'Joe'
        /// </remarks>
        public string DisplayMemberPath
        {
            get { return DisplayMemberPathField; }
            set
            {
                if (DisplayMemberPathField != value)
                {
                    OnPropertyChanging(EventArgSingletons.DisplayMemberPath);
                    DisplayMemberPathField = value;
                    OnPropertyChanged(EventArgSingletons.DisplayMemberPath);
                }

            }
        }
        private string DisplayMemberPathField = DefaultDisplayMemberPath;


        /// <summary>
        /// Binding for <see cref="ItemsControl.ItemsSource"/>
        /// </summary>
        public virtual E Items
        {
            get { return ItemSourceField; }
            set 
            {
                if (!(ItemSourceField?.Equals(value) ?? value is null))
                {
                    UnSubscribe(ItemSourceField);
                    OnPropertyChanging(EventArgSingletons.ItemSourceItems);
                    ItemSourceField = value;
                    OnPropertyChanged(EventArgSingletons.ItemSourceItems);
                    OnItemSourceChanged();
                    Subscribe(value);
                }
            }
        }

        IList IItemSource.Items { get => (IList)Items; }
        IList<T> IItemSource<T>.Items { get => Items; }

        private E ItemSourceField;

        private void UnSubscribe(E itemSource)
        {
            if (itemSource is INotifyCollectionChanged coll)
                coll.CollectionChanged -= RaiseItemSourceChanged;
        }

        private void Subscribe(E itemSource)
        {
            if (itemSource is INotifyCollectionChanged coll)
                coll.CollectionChanged += RaiseItemSourceChanged;
        }

        private void RaiseItemSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnItemSourceChanged(e);
        }

        #endregion
    }


}
