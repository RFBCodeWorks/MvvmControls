using RFBCodeWorks.MvvmControls;
using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.WPFBehaviors
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
                SetSelectorDefinition(cmb, args.NewValue as ISelector);
                if (args.OldValue is IComboBox oldDef)
                {
                    UnbindIfBound(cmb, ComboBox.IsDropDownOpenProperty, oldDef);
                    UnbindIfBound(cmb, ComboBox.IsReadOnlyProperty, oldDef);
                    UnbindIfBound(cmb, ComboBox.IsEditableProperty, oldDef);
                }
                if (args.NewValue is IComboBox def)
                {
                    SetBinding(cmb, ComboBox.IsDropDownOpenProperty, nameof(IComboBox.IsDropDownOpen), def);
                    SetBinding(cmb, ComboBox.IsReadOnlyProperty, nameof(IComboBox.IsReadOnly), def);
                    SetBinding(cmb, ComboBox.IsEditableProperty, nameof(IComboBox.IsEditable), def);
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
