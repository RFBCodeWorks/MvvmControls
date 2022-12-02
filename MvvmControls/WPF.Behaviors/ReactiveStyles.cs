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

        #region < IsTabletMode >

        /// <summary>
        /// Check the state of <see cref="IsTabletModeProperty"/>
        /// </summary>
        public static bool GetIsTabletMode(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsTabletModeProperty);
        }


        /// <summary>
        /// Set the state of <see cref="IsTabletModeProperty"/>
        /// </summary>
        public static void SetIsTabletMode(DependencyObject obj, bool value)
        {
            obj.SetValue(IsTabletModeProperty, value);
        }

        /// <summary>
        /// Enable/Disable the Reactive Style
        /// </summary>
        public static readonly DependencyProperty IsTabletModeProperty =
            DependencyProperty.RegisterAttached("IsTabletMode", typeof(bool), typeof(ReactiveStyles), new PropertyMetadata(false, UpdateElement));


        /// <summary>
        /// Gets the Style Setters for the 'Dirty' style
        /// </summary>
        public static SetterBaseCollection GetTabletModeSetters(DependencyObject obj)
        {
            return (SetterBaseCollection)obj.GetValue(TabletModeSettersProperty);
        }

        /// <summary>
        /// Sets the Style Setters for the 'Dirty' style
        /// </summary>
        public static void SetTabletModeSetters(DependencyObject obj, SetterBaseCollection value)
        {
            obj.SetValue(TabletModeSettersProperty, value);
        }

        /// <summary>
        /// The collection of style setters to enable when the <see cref="IsDirtyProperty"/> is set TRUE
        /// </summary>
        public static readonly DependencyProperty TabletModeSettersProperty =
            DependencyProperty.RegisterAttached("TabletModeSetters", typeof(SetterBaseCollection), typeof(ReactiveStyles), new PropertyMetadata(default));

        private static readonly DependencyProperty IsTabletModeActiveProperty =
            DependencyProperty.RegisterAttached("IsTableModeActive", typeof(bool), typeof(ReactiveStyles), new PropertyMetadata(false));
        private static bool IsTabletModeActive(DependencyObject obj) => (bool)obj.GetValue(IsTabletModeActiveProperty);

        #endregion

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
        /// </summary>
        public static readonly DependencyProperty IsDirtyProperty =
            DependencyProperty.RegisterAttached("IsDirty", typeof(bool), typeof(ReactiveStyles), new PropertyMetadata(false, UpdateElement));


        /// <summary>
        /// Gets the Style Setters for the 'Dirty' style
        /// </summary>
        public static SetterBaseCollection GetIsDirtySetters(DependencyObject obj)
        {
            return (SetterBaseCollection)obj.GetValue(IsDirtySettersProperty);
        }

        /// <summary>
        /// Sets the Style Setters for the 'Dirty' style
        /// </summary>
        public static void SetIsDirtySetters(DependencyObject obj, SetterBaseCollection value)
        {
            obj.SetValue(IsDirtySettersProperty, value);
        }

        /// <summary>
        /// The collection of style setters to enable when the <see cref="IsDirtyProperty"/> is set TRUE
        /// </summary>
        public static readonly DependencyProperty IsDirtySettersProperty =
            DependencyProperty.RegisterAttached("IsDirtySetters", typeof(SetterBaseCollection), typeof(ReactiveStyles), new PropertyMetadata(default));

        private static readonly DependencyProperty IsDirtyStyleActiveProperty =
            DependencyProperty.RegisterAttached("IsDirtyStyleActive", typeof(bool), typeof(ReactiveStyles), new PropertyMetadata(false));
        private static bool IsDirtyStyleActive(DependencyObject obj) => (bool)obj.GetValue(IsDirtyStyleActiveProperty);

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
            DependencyProperty.RegisterAttached("IsInvalid", typeof(bool), typeof(ReactiveStyles), new PropertyMetadata(false, propertyChangedCallback: UpdateElement));

        /// <summary>
        /// Gets the Style Setters for the 'IsInvalid' style
        /// </summary>
        public static SetterBaseCollection GetIsInvalidSetters(DependencyObject obj)
        {
            return (SetterBaseCollection)obj.GetValue(IsInvalidSettersProperty);
        }

        /// <summary>
        /// Sets the Style Setters for the 'IsInvalid' style
        /// </summary>
        public static void SetIsInvalidSetters(DependencyObject obj, SetterBaseCollection value)
        {
            obj.SetValue(IsInvalidSettersProperty, value);
        }

        /// <summary>
        /// The collection of style setters to enable when the <see cref="IsInvalidProperty"/> is set <see langword="true"/>
        /// </summary>
        public static readonly DependencyProperty IsInvalidSettersProperty =
            DependencyProperty.RegisterAttached("IsInvalidSetters", typeof(SetterBaseCollection), typeof(ReactiveStyles), new PropertyMetadata(default));

        private static readonly DependencyProperty IsInvalidStyleActiveProperty =
            DependencyProperty.RegisterAttached("IsInvalidStyleActive", typeof(bool), typeof(ReactiveStyles), new PropertyMetadata(false));
        private static bool IsInvalidStyleActive(DependencyObject obj) => (bool)obj.GetValue(IsInvalidStyleActiveProperty);

        #endregion

        /// <summary>
        /// Apply/remove the styles to/from the element
        /// </summary>
        private static void UpdateElement(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element) return;
            
            // Check if any styles should be applied. If not, apply the base style and return
            bool isTabletMode = GetIsTabletMode(element);
            bool isDirty = GetIsDirty(element);
            bool isInvalid = GetIsInvalid(element);

            ////Abort if no style change is needed
            //if (
            //    isTabletMode == IsTabletModeActive(element) &&
            //    isDirty == IsDirtyStyleActive(element) &&
            //    isInvalid == IsInvalidStyleActive(element)
            //    )
            //    return;

            //Get Base Style
            Style baseStyle;
            if (IsTabletModeActive(element) || IsDirtyStyleActive(element) || IsInvalidStyleActive(element))
            {
                baseStyle = element.Style.BasedOn;
            }
            else
            {
                baseStyle = element.Style;
            }

            // Update the properties storing if the style is active
            element.SetValue(IsTabletModeActiveProperty, isTabletMode);
            element.SetValue(IsDirtyStyleActiveProperty, isDirty);
            element.SetValue(IsInvalidStyleActiveProperty, isInvalid);

            // Restore the base style if no superceding style is required, then exit
            if (!isDirty && !isInvalid && !isTabletMode)
            {
                element.Style = baseStyle;
                return;
            }

            //Create a new style and apply the styles in order of importance
            Style responsivenessStyle = new Style(element.GetType(), baseStyle);
            if (isTabletMode)
            {
                _ = ApplyStyleSetters(responsivenessStyle, GetTabletModeSetters(element));
            }
            if (isDirty)
            {
                _ = ApplyStyleSetters(responsivenessStyle, GetIsDirtySetters(element));
            }
            if (isInvalid)
            {
                _ = ApplyStyleSetters(responsivenessStyle, GetIsInvalidSetters(element));
            }

            // Apply the style
            element.Style = responsivenessStyle;
        }

        /// <summary>
        /// Applies the setters to the style
        /// </summary>
        /// <param name="style">The style to apply the setters to</param>
        /// <param name="collection">The style setters collection</param>
        /// <returns>The input <paramref name="style"/>, after it has had the setters applied</returns>
        private static Style ApplyStyleSetters(Style style, SetterBaseCollection collection)
        {
            if (collection is null) return style;
            foreach (Setter setter in collection)
            {
                style.Setters.Add(setter);
            }
            return style;
        }
    }
}
