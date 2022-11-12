using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.MvvmControls.Behaviors
{
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Assigns an <see cref="IComboBox"/> to a <see cref="ComboBox"/>
        /// </summary>
        public static readonly DependencyProperty ComboBoxDefinitionProperty =
            DependencyProperty.RegisterAttached("ComboBoxDefinition",
                typeof(IComboBox),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, ComboBoxDefinitionPropertyChanged)
                );

        private static void ComboBoxDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ComboBox cmb)
            {
                if (args.OldValue is IComboBox oldDef)
                {
                    UnbindSelectorDefinition(cmb, oldDef);
                    UnbindIfBound(cmb, ComboBox.IsDropDownOpenProperty, oldDef);
                }
                if (args.NewValue is IComboBox def)
                {
                    BindSelectorDefinition(cmb, def);
                    SetBinding(cmb, ComboBox.IsDropDownOpenProperty, nameof(IComboBox.IsDropDownOpen), def);
                }
            }
        }

        /// <summary>
        /// Gets the assigned <see cref="IComboBox"/> from a <see cref="ComboBox"/>
        /// </summary>
        public static IComboBox GetComboBoxDefinition(DependencyObject obj)
        {
            return (IComboBox)obj.GetValue(ComboBoxDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IComboBox"/> to a <see cref="ComboBox"/>
        /// </summary>
        public static void SetComboBoxDefinition(DependencyObject obj, IComboBox value)
        {
            obj.SetValue(ComboBoxDefinitionProperty, value);
        }

    }
}
