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







        public static TriggerObject GetDirtyTrigger(DependencyObject obj)
        {
            return (TriggerObject)obj.GetValue(DirtyTriggerProperty);
        }

        public static void SetDirtyTrigger(DependencyObject obj, TriggerObject value)
        {
            obj.SetValue(DirtyTriggerProperty, value);
        }

        // Using a DependencyProperty as the backing store for DirtyTrigger.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DirtyTriggerProperty =
            DependencyProperty.RegisterAttached("DirtyTrigger", typeof(TriggerObject), typeof(ReactiveStyles), new PropertyMetadata(null));







    }


    public class TriggerObject : FrameworkElement
    {


        public object Value
        {
            get { return (object)GetValue(TestProperty); }
            set { SetValue(TestProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Test.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TestProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(TriggerObject), new PropertyMetadata(null));


    }

}

