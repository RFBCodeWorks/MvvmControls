using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RFBCodeWorks.MvvmControls.Behaviors
{
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Assigns an <see cref="IProgressBarDefinition"/> to a <see cref="ProgressBar"/>
        /// </summary>
        public static readonly DependencyProperty ProgressBarDefinitionProperty =
            DependencyProperty.RegisterAttached("ProgressBarDefinition",
                typeof(IProgressBarDefinition),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, ProgressBarDefinitionPropertyChanged)
                );

        private static void ProgressBarDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ProgressBar cmb)
            {
                if (args.OldValue is IProgressBarDefinition oldDef)
                {
                    UnbindIControlDefinition(cmb, oldDef);
                    UnbindIfBound(cmb, ProgressBar.ValueProperty, oldDef);
                    UnbindIfBound(cmb, ProgressBar.MaximumProperty, oldDef);
                    UnbindIfBound(cmb, ProgressBar.MinimumProperty, oldDef);
                    UnbindIfBound(cmb, ProgressBar.IsIndeterminateProperty, oldDef);
                }
                if (args.NewValue is IProgressBarDefinition def)
                {
                    BindIControlDefinition(cmb, def);
                    SetBinding(cmb, ProgressBar.ValueProperty, nameof(def.Value), def, BindingMode.OneWay);
                    SetBinding(cmb, ProgressBar.MinimumProperty, nameof(def.Minimum), def, BindingMode.OneWay);
                    SetBinding(cmb, ProgressBar.MaximumProperty, nameof(def.Maximum), def, BindingMode.OneWay);
                    SetBinding(cmb, ProgressBar.IsIndeterminateProperty, nameof(def.IsIndeterminate), def, BindingMode.OneWay);
                }
            }
        }

        /// <summary>
        /// Gets the assigned <see cref="IProgressBarDefinition"/> from a <see cref="ProgressBar"/>
        /// </summary>
        public static IProgressBarDefinition GetProgressBarDefinition(DependencyObject obj)
        {
            return (IProgressBarDefinition)obj.GetValue(ProgressBarDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IComboBoxDefinition"/> to a <see cref="ProgressBar"/>
        /// </summary>
        public static void SetProgressBarDefinition(DependencyObject obj, IProgressBarDefinition value)
        {
            obj.SetValue(ProgressBarDefinitionProperty, value);
        }
    }
}
