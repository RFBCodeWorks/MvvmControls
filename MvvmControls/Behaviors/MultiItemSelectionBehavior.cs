using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RFBCodeWorks.MvvmControls.Behaviors.Base;
using RFBCodeWorks.MvvmControls.ControlInterfaces;

namespace RFBCodeWorks.MvvmControls.Behaviors
{

    /// <summary>
    /// Allows two-way binding to ListBox.SelectedItems for listboxes with a object type of int
    /// </summary>
    public class MultiItemSelectionBehaviorInt : MultiItemSelectionBehavior<int> { }

    /// <summary>
    /// Allows two-way binding to ListBox.SelectedItems for listboxes with a object type of string
    /// </summary>
    public class MultiItemSelectionBehaviorString : MultiItemSelectionBehavior<string> { }

    /// <summary>
    /// Allows two-way binding to ListBox.SelectedItems for listboxes with a object type of FileInfo
    /// </summary>
    public class MultiItemSelectionBehaviorFileInfo : MultiItemSelectionBehavior<FileInfo> { }

    /// <summary>
    /// Allows two-way binding to ListBox.SelectedItems for listboxes with a object type of FileInfo
    /// </summary>
    public class MultiItemSelectionBehaviorDirectoryInfo : MultiItemSelectionBehavior<DirectoryInfo> { }

    /// <summary>
    /// Allows two-way binding to ListBox.SelectedItems for listboxes with a object type of FileInfo
    /// </summary>
    public class MultiItemSelectionBehaviorDriveInfo : MultiItemSelectionBehavior<DriveInfo> { }
}

namespace RFBCodeWorks.MvvmControls.Behaviors.Base
{

    /// <summary>
    /// Generic Class that allows two-way binding to the ListBox.SelectedItems property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks><see href="https://tyrrrz.me/blog/wpf-listbox-selecteditems-twoway-binding"/></remarks>
    public class MultiItemSelectionBehavior<T> : Behavior<ListBox>
    {
        /// <summary>
        /// Attached Dependency Property for the behavior
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList<T>),
                typeof(MultiItemSelectionBehavior<T>),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemsChanged));

        private bool _viewHandled;
        private bool _modelHandled;

        /// <summary>
        /// Represents the SelectedItems property of the ViewModel
        /// </summary>
        public IList<T> SelectedItems
        {
            get => (IList<T>)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        // Raised when the  DP Property is added to initialize the selected items on the view
        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var behavior = (MultiItemSelectionBehavior<T>)sender;
            if (behavior._modelHandled) return;

            if (behavior.AssociatedObject == null)
                return;

            behavior._modelHandled = true;
            behavior.SelectItems();
            behavior._modelHandled = false;
        }

        // Propagate selected items from model to view
        private void SelectItems()
        {
            _viewHandled = true;
            AssociatedObject.SelectedItems.Clear();
            if (SelectedItems != null)
            {
                foreach (var item in SelectedItems)
                    AssociatedObject.SelectedItems.Add(item);
            }
            _viewHandled = false;
        }

        // Propagate selected items from view to model
        private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (_viewHandled) return;
            if (AssociatedObject.Items.SourceCollection == null) return;

            //Get the selected objects of the specified type and assign it to the model
            var arr = (T[])Array.CreateInstance(typeof(T), AssociatedObject.SelectedItems.Count);
            int i = 0;
            foreach(object o in AssociatedObject.SelectedItems)
            {
                if (o is T item)
                {
                    arr[i] = item;
                    i++;
                }
            }
            _modelHandled = true;
            SelectedItems = arr.ToList();
            _modelHandled = false;
        }

        // Re-select items when the set of items changes
        private void OnListBoxItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (_viewHandled) return;
            if (AssociatedObject.Items.SourceCollection == null) return;

            SelectItems();
        }
        
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectionChanged += OnListBoxSelectionChanged;
            ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged += OnListBoxItemsChanged;
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.SelectionChanged -= OnListBoxSelectionChanged;
                ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged -= OnListBoxItemsChanged;
            }
        }
    }
}
