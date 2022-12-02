using RFBCodeWorks.MvvmControls;
using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.WPF.Behaviors
{
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Assigns an <see cref="IRadioButton"/> to a <see cref="RadioButton"/>
        /// </summary>
        public static readonly DependencyProperty RadioButtonDefinitionProperty =
            DependencyProperty.RegisterAttached("RadioButtonDefinition",
                typeof(IRadioButton),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, RadioButtonDefinitionPropertyChanged)
                );

        private static void RadioButtonDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is RadioButton btn)
            {
                if (args.OldValue is IRadioButton oldDef)
                {
                    UnbindIfBound(btn, RadioButton.GroupNameProperty, oldDef);
                }

                SetToggleButtonDefinition(btn, args.NewValue as IToggleButton);

                if (args.NewValue is IRadioButton def)
                {
                    SetBinding(btn, RadioButton.GroupNameProperty, nameof(def.GroupName), def);
                }
            }
        }

        /// <summary>
        /// Gets the assigned <see cref="IRadioButton"/> from a <see cref="ComboBox"/>
        /// </summary>
        public static IRadioButton GetRadioButtonDefinition(DependencyObject obj)
        {
            return (IRadioButton)obj.GetValue(RadioButtonDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IRadioButton"/> to a <see cref="ComboBox"/>
        /// </summary>
        public static void SetRadioButtonDefinition(DependencyObject obj, IRadioButton value)
        {
            obj.SetValue(RadioButtonDefinitionProperty, value);
        }
    }
}
