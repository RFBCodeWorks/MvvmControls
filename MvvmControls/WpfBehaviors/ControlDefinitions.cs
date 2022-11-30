using RFBCodeWorks.MvvmControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RFBCodeWorks.WPFBehaviors
{
    /// <summary>
    /// Contains the AttachedProperties to bind controls to their definitions
    /// </summary>
    public static partial class ControlDefinitions
    {

        internal static Binding GetBindingInfo(DependencyObject obj, DependencyProperty prop)
        {
            return BindingOperations.GetBindingExpression(obj, prop)?.ParentBinding;
        }

        /// <summary>
        /// Unbind the specified dependency property if the source is the same as the binding.source
        /// </summary>
        /// <param name="obj">the bound control</param>
        /// <param name="prop">the bound dependencyproperty</param>
        /// <param name="source">The control definition</param>
        internal static void UnbindIfBound<T>(DependencyObject obj, DependencyProperty prop, T source)
            where T : class, IControlDefinition
        {
            var binding = BindingOperations.GetBindingExpression(obj, prop)?.ParentBinding;
            if ((binding?.Source is null) || binding.Source as string == "") return;
            if (binding.Source as T == source) 
                BindingOperations.ClearBinding(obj, Control.VisibilityProperty);
        }

        /// <summary>
        /// Unbind the specified dependency property if the source is the same as the binding.source
        /// </summary>
        /// <param name="obj">the bound control</param>
        /// <param name="prop">the bound dependencyproperty</param>
        /// <param name="source">The control definition</param>
        /// <param name="propertyName">The name of the bound property. This must be a property of the <typeparamref name="T"/> <paramref name="source"/></param>
        /// <param name="mode">The binding mode - if not specified uses the default for this property</param>
        /// <param name="trigger">The update trigger</param>
        /// <returns>The binding that was created</returns>
        /// <typeparam name="T">The IControlDefinition interface type</typeparam>
        internal static void SetBinding<T>(DependencyObject obj, DependencyProperty prop, string propertyName, T source,
            BindingMode? mode = null,
            UpdateSourceTrigger? trigger = null)
        where T : class, IControlDefinition
        {
            BindingOperations.SetBinding(obj, prop, new Binding(propertyName)
            {
                Source = source,
                Mode = mode ?? BindingMode.Default,
                UpdateSourceTrigger = trigger ?? UpdateSourceTrigger.Default
            });
        }

        /// <summary>
        /// Bind to Visibility and ToolTip
        /// </summary>
        private static void BindIControlDefinition(Control obj, IControlDefinition def)
        {
            //Visibility
            SetBinding(obj, UIElement.VisibilityProperty, nameof(IControlDefinition.Visibility), def, BindingMode.OneWay);
            SetBinding(obj, FrameworkElement.ToolTipProperty, nameof(IControlDefinition.ToolTip), def, BindingMode.OneWay);
            SetBinding(obj, FrameworkElement.IsEnabledProperty, nameof(IControlDefinition.IsEnabled), def, BindingMode.OneWay);
        }

        /// <summary>
        /// Unbind Visibiltiy and ToolTip
        /// </summary>
        private static void UnbindIControlDefinition(Control obj, IControlDefinition def)
        {
            UnbindIfBound(obj, UIElement.VisibilityProperty, def);
            UnbindIfBound(obj, FrameworkElement.ToolTipProperty, def);
            UnbindIfBound(obj, FrameworkElement.IsEnabledProperty, def);
        }
    }
}
