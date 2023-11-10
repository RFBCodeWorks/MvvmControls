using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.WPF.Behaviors
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Helper class to set <see cref="Behaviors.ReactiveStyles.IsDirtyProperty"/> when the Default and Current values don't match
    /// </summary>
    public static class ReactiveStylesIsDirtyString
    {

        public static bool GetEnableComparer(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableComparerProperty);
        }

        public static void SetEnableComparer(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableComparerProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnableComparer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableComparerProperty =
            DependencyProperty.RegisterAttached("EnableComparer", typeof(bool), typeof(ReactiveStylesIsDirtyString), new PropertyMetadata(true, IsEnabledChanged));


        public static string GetOriginalValue(DependencyObject obj)
        {
            return (string)obj.GetValue(OriginalValueProperty);
        }

        public static void SetOriginalValue(DependencyObject obj, string value)
        {
            obj.SetValue(OriginalValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for OriginalValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginalValueProperty =
            DependencyProperty.RegisterAttached("OriginalValue", typeof(string), typeof(ReactiveStylesIsDirtyString), new PropertyMetadata(default, CompareValues));


        public static string GetCurrentValue(DependencyObject obj)
        {
            return (string)obj.GetValue(CurrentValueProperty);
        }

        public static void SetCurrentValue(DependencyObject obj, string value)
        {
            obj.SetValue(CurrentValueProperty, value);
        }


        // Using a DependencyProperty as the backing store for CurrentValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.RegisterAttached("CurrentValue", typeof(string), typeof(ReactiveStylesIsDirtyString), new PropertyMetadata(default, CompareValues));


        /// <summary>
        /// Compare the two values
        /// </summary>
        private static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is true && e.NewValue is false)
            {
                d.SetValue(ReactiveStyles.IsDirtyProperty, false);
            }
            else if (e.NewValue is true)
            {
                CompareValues(d, e);
            }
        }

        /// <summary>
        /// Compare the two values, and set IsDirty
        /// </summary>
        private static void CompareValues(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!GetEnableComparer(d)) return;
            bool isDirty = !GetOriginalValue(d).Equals(GetCurrentValue(d));
            d.SetValue(ReactiveStyles.IsDirtyProperty, isDirty);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
