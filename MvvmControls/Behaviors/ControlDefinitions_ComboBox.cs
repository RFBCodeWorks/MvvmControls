using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.MvvmControls.Behaviors
{
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Assigns an <see cref="IComboBoxDefinition"/> to a <see cref="ComboBox"/>
        /// </summary>
        public static readonly DependencyProperty ComboBoxDefinitionProperty =
            DependencyProperty.RegisterAttached("ComboBoxDefinition",
                typeof(IComboBoxDefinition),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, ComboBoxDefinitionPropertyChanged)
                );

        private static void ComboBoxDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ComboBox cmb)
            {
                if (args.OldValue is IComboBoxDefinition oldDef)
                {
                    UnbindSelectorDefinition(cmb, oldDef);
                    UnbindIfBound(cmb, ComboBox.IsDropDownOpenProperty, oldDef);
                }
                if (args.NewValue is IComboBoxDefinition def)
                {
                    BindSelectorDefinition(cmb, def);
                    SetBinding(cmb, ComboBox.IsDropDownOpenProperty, nameof(IComboBoxDefinition.IsDropDownOpen), def);
                }
            }
        }

        /// <summary>
        /// Gets the assigned <see cref="IComboBoxDefinition"/> from a <see cref="ComboBox"/>
        /// </summary>
        public static IComboBoxDefinition GetComboBoxDefinition(DependencyObject obj)
        {
            return (IComboBoxDefinition)obj.GetValue(ComboBoxDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IComboBoxDefinition"/> to a <see cref="ComboBox"/>
        /// </summary>
        public static void SetComboBoxDefinition(DependencyObject obj, IComboBoxDefinition value)
        {
            obj.SetValue(ComboBoxDefinitionProperty, value);
        }

    }
}
