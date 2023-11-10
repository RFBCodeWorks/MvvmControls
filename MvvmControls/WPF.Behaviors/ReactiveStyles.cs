using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.WPF.Behaviors
{

    /// <summary>
    /// Contains Attached properties for triggering a style to overlay onto a control
    /// </summary>
    public static partial class ReactiveStyles
    {

        #region < IsDirty >

        /// <summary>
        /// Check the state of <see cref="IsDirtyProperty"/>
        /// </summary>
        public static bool GetIsDirty(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDirtyProperty);
        }


        /// <summary>
        /// Set the state of <see cref="IsDirtyProperty"/>
        /// </summary>
        public static void SetIsDirty(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDirtyProperty, value);
        }

        /// <summary>
        /// Enable/Disable the 'Dirty' Style for the control
        /// <br/> When <see langword="true"/>, this would indicate that the value has been changed but has not yet been saved. (This should be set false after the value has been saved)
        /// </summary>
        public static readonly DependencyProperty IsDirtyProperty =
            DependencyProperty.RegisterAttached("IsDirty", typeof(bool), typeof(ReactiveStyles), new PropertyMetadata(false));


        #endregion

        #region < IsInvalid >

        /// <summary>
        /// Check the state of <see cref="IsInvalidProperty"/>
        /// </summary>
        public static bool GetIsInvalid(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsInvalidProperty);
        }

        /// <summary>
        /// Check the state of <see cref="IsInvalidProperty"/>
        /// </summary>
        public static void SetIsInvalid(DependencyObject obj, bool value)
        {
            obj.SetValue(IsInvalidProperty, value);
        }

        /// <summary>
        /// Enable/Disable the 'IsInvalid' style for the control
        /// </summary>
        public static readonly DependencyProperty IsInvalidProperty =
            DependencyProperty.RegisterAttached("IsInvalid", typeof(bool), typeof(ReactiveStyles), new PropertyMetadata(false));

        #endregion

    }

}

