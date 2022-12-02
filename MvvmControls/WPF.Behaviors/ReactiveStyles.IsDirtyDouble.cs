using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.WPF.Behaviors.Helpers
{
    /// <summary>
    /// Helper class to set <see cref="Behaviors.ReactiveStyles.IsDirtyProperty"/> when the Default and Current values don't match
    /// </summary>
    public static class IsDirtyDouble
    {


        public static bool GetEnableDirtyComparer(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableDirtyComparerProperty);
        }

        public static void SetEnableDirtyComparer(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableDirtyComparerProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnableDirtyComparer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableDirtyComparerProperty =
            DependencyProperty.RegisterAttached("EnableDirtyComparer", typeof(bool), typeof(IsDirtyDouble), new PropertyMetadata(false, IsEnabledChanged));



        public static double GetDefaultValue(DependencyObject obj)
        {
            return (double)obj.GetValue(DefaultValueProperty);
        }

        public static void SetDefaultValue(DependencyObject obj, double value)
        {
            obj.SetValue(DefaultValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for DefaultValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.RegisterAttached("DefaultValue", typeof(double), typeof(IsDirtyDouble), new PropertyMetadata(default, CompareValues));


        public static double GetCurrentValue(DependencyObject obj)
        {
            return (double)obj.GetValue(CurrentValueProperty);
        }

        public static void SetCurrentValue(DependencyObject obj, double value)
        {
            obj.SetValue(CurrentValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for CurrentValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.RegisterAttached("CurrentValue", typeof(double), typeof(IsDirtyDouble), new PropertyMetadata(default, CompareValues));


        private static readonly DependencyProperty IsDirtyComparerActiveProperty =
        DependencyProperty.RegisterAttached("IsDirtyComparerActive", typeof(bool), typeof(ReactiveStyles), new PropertyMetadata(false));
        private static bool IsDirtyComparerActive(DependencyObject obj) => (bool)obj.GetValue(IsDirtyComparerActiveProperty);


        /// <summary>
        /// Compare the two values
        /// </summary>
        private static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (IsDirtyComparerActive(d))
            {

            }
            else
            {

            }
            d.SetValue(IsDirtyComparerActiveProperty, e.NewValue);
        }

        /// <summary>
        /// Compare the two values, and set IsDirty
        /// </summary>
        private static void CompareValues(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!IsDirtyComparerActive(d)) return;
            bool isDirty = !GetDefaultValue(d).Equals(GetCurrentValue(d));
            d.SetValue(ReactiveStyles.IsDirtyProperty, isDirty);
        }
    }

}
