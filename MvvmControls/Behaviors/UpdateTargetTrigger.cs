using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using RFBCodeWorks.MvvmControls.ControlInterfaces;

namespace RFBCodeWorks.MvvmControls.Behaviors
{

    /// <summary>
    /// 
    /// </summary>
    public enum UpdateTargetTrigger
    {
        /// <summary>
        /// <inheritdoc cref="UpdateSourceTrigger.PropertyChanged"/>
        /// </summary>
        PropertyChanged,

        /// <summary>
        /// <inheritdoc cref="UpdateSourceTrigger.Explicit"/>
        /// </summary>
        Explicit,

        /// <summary>
        /// <inheritdoc cref="UpdateSourceTrigger.LostFocus"/>
        /// </summary>
        LostFocus,

        /// <summary>
        /// <inheritdoc cref="UpdateSourceTrigger.PropertyChanged"/>
        /// </summary>
        Default
    }

    /// <summary>
    /// Attached Property that causes a binding to refresh on some trigger
    /// </summary>
    public class UpdateTargetAttachedProperty
    {
        
        /// <summary>
        /// Event that occurs when the source is updated
        /// </summary>
        public static readonly RoutedEvent SourceUpdated = EventManager.RegisterRoutedEvent("SourceUpdated", RoutingStrategy.Direct, typeof(PropertyChangedCallback), typeof(DependencyProperty));

        /// <summary>
        /// The DependencyProperty to refresh when the trigger occurs
        /// </summary>
        public DependencyProperty TargetProperty { get; set; }

        /// <summary>
        /// The Attached Property
        /// </summary>
        public static readonly DependencyProperty UpdateTargetTriggerProperty =
            DependencyProperty.RegisterAttached(
                nameof(UpdateTargetTrigger),
                typeof(UpdateTargetTrigger),
                typeof(UpdateTargetAttachedProperty),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback((o, e) =>
                   {
                       // To Do - figure this out? no idewa where I left off.
                   })));

        /// <summary>
        /// Get the value
        /// </summary>
        public static UpdateTargetTrigger GetUpdateTargetProperty(FrameworkElement prop)
        {
            return (UpdateTargetTrigger)prop.GetValue(UpdateTargetTriggerProperty);
        }

        /// <summary>
        /// Set the value
        /// </summary>
        public static void SetUpdateTargetProperty(FrameworkElement prop, UpdateTargetAttachedProperty trigger)
        {
            prop.GetBindingExpression(trigger.TargetProperty).UpdateTarget();
            prop.SetValue(UpdateTargetTriggerProperty, trigger);
        }

        /// <summary>
        /// Updates the <see cref="TargetProperty"/>
        /// </summary>
        /// <param name="b"></param>
        /// <param name="trigger"></param>
        private static void RefreshBinding(BindingExpression b, UpdateTargetTrigger trigger)
        {
            bool OKToRefresh = b.ParentBinding.Mode switch
            {
                BindingMode.TwoWay => true,
                BindingMode.OneWay => true,
                BindingMode.OneTime => true,
                BindingMode.OneWayToSource => true,
                BindingMode.Default => true,
                _ => throw new NotImplementedException("Unknown type of System.Windows.Data.BindingMode object"),
            };

            OKToRefresh = OKToRefresh && trigger switch
            {
                UpdateTargetTrigger.Explicit => false,
                UpdateTargetTrigger.PropertyChanged => false,
                UpdateTargetTrigger.Default => false,
                UpdateTargetTrigger.LostFocus => false,
                _ => throw new NotImplementedException("Unknown type of UpdateTargetTrigger object"),
            };

            if (OKToRefresh)
            {
                b.UpdateTarget();
            }
        }

    }

}
