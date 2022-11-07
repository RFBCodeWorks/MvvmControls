using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using RFBCodeWorks.MvvmControls.ControlInterfaces;

namespace RFBCodeWorks.MvvmControls.Behaviors
{
    /// <summary>
    /// Contains the AttachedProperties to bind controls to their definitions
    /// </summary>
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Unbind the specified dependency property if the source is the same as the binding.source
        /// </summary>
        /// <param name="obj">the bound control</param>
        /// <param name="prop">the bound dependencyproperty</param>
        /// <param name="source">The control definition</param>
        private static void UnbindIfBound<T>(DependencyObject obj, DependencyProperty prop, T source)
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
        /// <returns>The binding that was created</returns>
        /// <typeparam name="T">The IControlDefinition interface type</typeparam>
        private static void SetBinding<T>(DependencyObject obj, DependencyProperty prop, string propertyName, T source, BindingMode? mode = null)
        where T : class, IControlDefinition
        {
            if (mode.HasValue)
            {
                BindingOperations.SetBinding(obj, prop, new Binding(propertyName)
                {
                    Source = source,
                    Mode = mode.Value
                });
            }
            else
            {
                BindingOperations.SetBinding(obj, prop, new Binding(propertyName)
                {
                    Source = source
                });
            }
            
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
