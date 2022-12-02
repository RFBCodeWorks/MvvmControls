using RFBCodeWorks.MvvmControls;
using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.WPF.Behaviors
{
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Assigns an <see cref="IComboBox"/> to a <see cref="CheckBox"/>
        /// </summary>
        public static readonly DependencyProperty CheckBoxDefinitionProperty =
            DependencyProperty.RegisterAttached("CheckBoxDefinition",
                typeof(ICheckBox),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, CheckBoxDefinitionPropertyChanged)
                );

        private static void CheckBoxDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is CheckBox cmb)
            {
                SetToggleButtonDefinition(cmb, args.NewValue as IToggleButton);
            }
        }

        /// <summary>
        /// Gets the assigned <see cref="ICheckBox"/> from a <see cref="CheckBox"/>
        /// </summary>
        public static ICheckBox GetCheckBoxDefinition(DependencyObject obj)
        {
            return (ICheckBox)obj.GetValue(CheckBoxDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IComboBox"/> to a <see cref="CheckBox"/>
        /// </summary>
        public static void SetCheckBoxDefinition(DependencyObject obj, ICheckBox value)
        {
            obj.SetValue(CheckBoxDefinitionProperty, value);
        }
    }
}
