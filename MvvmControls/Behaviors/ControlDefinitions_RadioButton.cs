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
    public static partial class ControlDefinitions
    {
        /// <summary>
        /// Assigns an <see cref="IRadioButtonDefinition"/> to a <see cref="RadioButton"/>
        /// </summary>
        public static readonly DependencyProperty RadioButtonDefinitionProperty =
            DependencyProperty.RegisterAttached("RadioButtonDefinition",
                typeof(IRadioButtonDefinition),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, RadioButtonDefinitionPropertyChanged)
                );

        private static void RadioButtonDefinitionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is RadioButton btn)
            {
                if (args.OldValue is IRadioButtonDefinition oldDef)
                {
                    if (GetToggleButtonDefinition(btn) == args.OldValue)
                        SetToggleButtonDefinition(btn, null);
                    else
                        UnbindToggleButtonDefinition(btn, oldDef);
                    UnbindIfBound(btn, RadioButton.GroupNameProperty, oldDef);
                }
                
                if (args.NewValue is IRadioButtonDefinition def)
                {
                    SetToggleButtonDefinition(btn, def);
                    SetBinding(btn, RadioButton.GroupNameProperty, nameof(def.GroupName), def);
                }
                SetContent(btn, args.NewValue);
            }
        }

        /// <summary>
        /// Gets the assigned <see cref="IRadioButtonDefinition"/> from a <see cref="ComboBox"/>
        /// </summary>
        public static IRadioButtonDefinition GetRadioButtonDefinition(DependencyObject obj)
        {
            return (IRadioButtonDefinition)obj.GetValue(RadioButtonDefinitionProperty);
        }

        /// <summary>
        /// Assigns an <see cref="IRadioButtonDefinition"/> to a <see cref="ComboBox"/>
        /// </summary>
        public static void SetRadioButtonDefinition(DependencyObject obj, IRadioButtonDefinition value)
        {
            obj.SetValue(RadioButtonDefinitionProperty, value);
        }
    }
}
