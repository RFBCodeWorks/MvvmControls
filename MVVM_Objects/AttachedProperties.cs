using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MVVMObjects
{

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

    public class UpdateTargetAttachedProperty
    {
        public UpdateTargetAttachedProperty()
        {

        }

        public static readonly RoutedEvent SourceUpdated = EventManager.RegisterRoutedEvent("SourceUpdated", RoutingStrategy.Direct, typeof(PropertyChangedCallback), typeof(DependencyProperty));

        public DependencyProperty TargetProperty { get; set; }


        public static readonly DependencyProperty UpdateTargetTriggerProperty =
            DependencyProperty.RegisterAttached(
                nameof(UpdateTargetTrigger),
                typeof(UpdateTargetTrigger),
                typeof(DependencyObject),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback((o, e) =>
                   {

                   })));

        public static UpdateTargetTrigger GetUpdateTargetProperty(FrameworkElement prop)
        {
            return (UpdateTargetTrigger)prop.GetValue(UpdateTargetTriggerProperty);
        }

        public static void SetUpdateTargetProperty(FrameworkElement prop, UpdateTargetAttachedProperty trigger)
        {



            prop.GetBindingExpression(trigger.TargetProperty).UpdateTarget();
            prop.SetValue(UpdateTargetTriggerProperty, trigger);
        }


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
